﻿namespace DatabaseComparison.Domain.Events
{
    [Serializable]
    public class CurrencyInfoAdded : IStoredEvent
    {
        public DateTime Time { get; set; }
        public int TickVolume { get; set; }
        public int RealVolume { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public DateTime ApiCallTime { get; set; }
        public DateTime SaveTime { get; set; }
    }
}
