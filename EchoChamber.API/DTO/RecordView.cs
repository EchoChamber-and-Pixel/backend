using System;

namespace EchoChamber.API.DTO
{
    public class RecordView
    {
        public int Id { get; set; }
        public string SteamID64 { get; set; }
        public string PlayerName { get; set; }
        public string MapName { get; set; }
        public int Course { get; set; }
        public int Style { get; set; }
        public int ModeId { get; set; }
        public string Mode => GetModeString(ModeId);
        public int Teleports { get; set; }
        public bool IsPro => Teleports == 0;
        public string Source { get; set; }
        public DateTime Created { get; set; }

        private string GetModeString(int mode)
        {
            switch (mode)
            {
                case 0:
                    return "VANILLA";
                case 1:
                    return "SIMPLEKZ";
                case 2:
                    return "KZTIMER";
                default:
                    return "UNKOWN - " + mode;
            }
        }
    }
}