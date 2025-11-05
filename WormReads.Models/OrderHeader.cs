using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WormReads.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        [ValidateNever]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User User { get; set; }


        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }


        public string? PaymentStatus { get; set; }
        public string? OrderStatus { get; set; }
        public string? Carrier {  get; set; }
        public string? TrackingNumber { get; set; }


        public DateTime PaymentDate { get; set; }//for companies because they have 30 days to pay
        public DateOnly PaymentDueDate { get; set; }

        public string? SessionId { get; set; } //the session Id from Stripe
        public string? PaymentIntentId { get; set; }//the ID that will be returned from the stripe service


        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
