using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class ItemTracker
    {
        public int Id { get; set; }
        public int Owner { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "User")]
        public int ApplicationUserId { get; set; }

        internal static string ReferenceEquals(string trackingId)
        {
            throw new NotImplementedException();
        }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public static string TrackingId;
        public ItemTracker()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[20];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var trackingId = finalString;

            //TrackingNumber = TrackingId;
            TrackingNumber = trackingId;
        }
    }
}
