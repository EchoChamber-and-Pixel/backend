using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace EchoChamber.API.Data
{
    internal enum GlobalMode
    {
        Vanilla = 0,
        KzSimple = 1,
        KzTimer = 2
    }

    internal enum GlobalStyle
    {
        Normal = 0
    }

    [Flags]
    internal enum EntityFlag
    {
        OnGround = 1 << 0,
        Ducking = 1 << 1,
        WaterJump = 1 << 2,
        OnTrain = 1 << 3,
        InRain = 1 << 4,
        Frozen = 1 << 5,
        AtControls = 1 << 6,
        Client = 1 << 7,
        FakeClient = 1 << 8,
        InWater = 1 << 9,
        Fly = 1 << 10,
        Swim = 1 << 11,
        Conveyor = 1 << 12,
        Npc = 1 << 13,
        GodMode = 1 << 14,
        NoTarget = 1 << 15,
        AimTarget = 1 << 16,
        PartialGround = 1 << 17,
        StaticProp = 1 << 18,
        Graphed = 1 << 19,
        Grenade = 1 << 20,
        StepMovement = 1 << 21,
        DontTouch = 1 << 22,
        BaseVelocity = 1 << 23,
        WorldBrush = 1 << 24,
        Object = 1 << 25,
        KillMe = 1 << 26,
        OnFire = 1 << 27,
        Dissolving = 1 << 28,
        TransRagdoll = 1 << 29,
        UnblockableByPlayer = 1 << 30,
        Freezing = 1 << 31
    }

    [Flags]
    internal enum Button
    {
        Attack = 1 << 0,
        Jump = 1 << 1,
        Duck = 1 << 2,
        Forward = 1 << 3,
        Back = 1 << 4,
        Use = 1 << 5,
        Cancel = 1 << 6,
        Left = 1 << 7,
        Right = 1 << 8,
        MoveLeft = 1 << 9,
        MoveRight = 1 << 10,
        Attack2 = 1 << 11,
        Run = 1 << 12,
        Reload = 1 << 13,
        Alt1 = 1 << 14,
        Alt2 = 1 << 15,
        Score = 1 << 16,
        Speed = 1 << 17,
        Walk = 1 << 18,
        Zoom = 1 << 19,
        Weapon1 = 1 << 20,
        Weapon2 = 1 << 21,
        BullRush = 1 << 22, // ...what?
        Grenade1 = 1 << 23,
        Grenade2 = 1 << 24
    }

    internal struct TickData
    {
        public Vector3 Position;
        public Vector2 Angles;
        public Button Buttons;
        public EntityFlag Flags;
    }

    internal class ReplayFile
    {
        public static readonly uint Magic = 0x676F6B7A;

        public byte FormatVersion;
        public string PluginVersion;

        public string MapName;
        public int Course;
        public GlobalMode Mode;
        public GlobalStyle Style;
        public TimeSpan Time;
        public int TeleportsUsed;
        public int SteamId;
        public string SteamId2;
        public string PlayerName;
        public int TickRate;

        public readonly List<TickData> Ticks = new List<TickData>();

        public ReplayFile()
        {
        }

        public ReplayFile(Stream stream, bool leaveOpen = false)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen);

            var magic = reader.ReadUInt32();
            if (magic != Magic)
            {
                throw new Exception("Unrecognised replay file format.");
            }

            FormatVersion = reader.ReadByte();
            PluginVersion = reader.ReadSmString();

            MapName = reader.ReadSmString();
            Course = reader.ReadInt32();
            Mode = (GlobalMode)reader.ReadInt32();
            Style = (GlobalStyle)reader.ReadInt32();
            Time = TimeSpan.FromSeconds(reader.ReadSingle());
            TeleportsUsed = reader.ReadInt32();
            SteamId = reader.ReadInt32();
            SteamId2 = reader.ReadSmString();
            reader.ReadSmString();
            PlayerName = reader.ReadSmString();
            var tickCount = reader.ReadInt32();
            TickRate = (int)Math.Round(tickCount / Time.TotalSeconds);

            Ticks.Clear();

            for (var i = 0; i < tickCount; ++i)
            {
                Ticks.Add(new TickData
                {
                    Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    Angles = new Vector2(reader.ReadSingle(), reader.ReadSingle()),
                    Buttons = (Button)reader.ReadInt32(),
                    Flags = (EntityFlag)reader.ReadInt32()
                });
            }
        }

        public void Write(Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

            writer.Write(Magic);

            writer.Write(FormatVersion);
            writer.Write(PluginVersion);

            writer.Write(MapName);
            writer.Write(Course);
            writer.Write((int)Mode);
            writer.Write((int)Style);
            writer.Write((float)Time.TotalSeconds);
            writer.Write(TeleportsUsed);
            writer.Write(SteamId);
            writer.Write(SteamId2);
            writer.Write("");
            writer.Write(PlayerName);
            writer.Write(Ticks.Count);

            if (Ticks != null)
            {
                foreach (var tick in Ticks)
                {
                    writer.Write(tick.Position.X);
                    writer.Write(tick.Position.Y);
                    writer.Write(tick.Position.Z);

                    writer.Write(tick.Angles.X);
                    writer.Write(tick.Angles.Y);

                    writer.Write((int)tick.Buttons);
                    writer.Write((int)tick.Flags);
                }
            }

            writer.Flush();
        }

        public static bool TryParse(Stream stream, out ReplayFile file)
        {
            try
            {
                file = new ReplayFile(stream);
                return true;
            }
            catch
            {
                file = null;
                return false;
            }
        }
    }

    internal static class Utils
    {
        public static string ReadSmString(this BinaryReader reader)
        {
            var length = reader.ReadByte();
            var bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public static TimeSpan ParseTime(string time)
        {
            var minuteSplit = time.IndexOf(':');
            var minutes = int.Parse(time.Substring(0, minuteSplit));
            var seconds = double.Parse(time.Substring(minuteSplit + 1));

            return TimeSpan.FromSeconds((minutes * 60) + seconds);
        }
    }
}