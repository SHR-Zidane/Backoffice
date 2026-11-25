namespace tounsiYoyo.Model
{
    public static class OpeningHours
    {
        // Créneaux d'ouverture par jour (heure d'ouverture incluse, heure de fermeture exclue).
        // Les jours absents (vendredi, dimanche) sont fermés.
        private static readonly Dictionary<DayOfWeek, (TimeSpan Open, TimeSpan Close)> Slots = new()
        {
            [DayOfWeek.Monday] = (TimeSpan.FromHours(10), TimeSpan.FromHours(22)),
            [DayOfWeek.Tuesday] = (TimeSpan.FromHours(12), TimeSpan.FromHours(22)),
            [DayOfWeek.Wednesday] = (TimeSpan.FromHours(12), TimeSpan.FromHours(22)),
            [DayOfWeek.Thursday] = (TimeSpan.FromHours(10), TimeSpan.FromHours(20)),
            [DayOfWeek.Saturday] = (TimeSpan.FromHours(12), TimeSpan.FromHours(20)),
        };

        public static bool IsOpen(DateTime localDateTime)
        {
            return Slots.TryGetValue(localDateTime.DayOfWeek, out var slot)
                && localDateTime.TimeOfDay >= slot.Open
                && localDateTime.TimeOfDay < slot.Close;
        }
    }
}
