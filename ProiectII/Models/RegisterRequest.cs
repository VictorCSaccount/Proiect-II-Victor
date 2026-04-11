using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    // am implementat aceasta clasa pentru ca am inteles de la colegii mai mari ca domnul Gota se folosea de SQL injection sa sparga baza dd edate!!
    // si nu cuosc daca si doamneel e de acum incearca sa ti sparga baza de date

    //extended protection from SQL injection
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)] // Testerul nu va putea pune parole de 2 caractere
        public string Password { get; set; }

    }
}
