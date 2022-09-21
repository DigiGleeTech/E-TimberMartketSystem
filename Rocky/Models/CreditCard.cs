using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rocky.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        [Required]
        public string Ownner { get; set; }

        [Required(ErrorMessage = "Please choose a card type")]
        public CardType Type { get; set; }

        [Required(ErrorMessage = "Card Number Is required Please")]
        [Display(Name = "Card Number")]
        [Range(1,16, ErrorMessage = "Invalid Card Number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry Is required Please")]
        [Display(Name = "Expiry")]

        public int Expiry { get; set; }

        [Required(ErrorMessage = "CCV Is required Please")]
        [Display(Name = "CCV")]
        [Range(1, 3, ErrorMessage = "Invalid CCV")]
        public int Cvv { get; set; }

        [Required(ErrorMessage = "Pin Is required Please")]
        [Display(Name = "Pin")]
        [Range(1, 4, ErrorMessage = "Invalid pin")]

        public int Pin { get; set; }

        [Required]
        public string UserId { get; set; }
        public Order Order { get; set; }
    }

    public enum CardType
    {
        Visa,
        MaterCard
    }
}
