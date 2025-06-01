using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.CrossCutting.Validation
{
    public class Error(string code, string description)
    {
        public string Code { get; set; } = code;
        public string Description { get; set; } = description;

        public ProblemDetails ToProblemDetails()
        {
            return new ProblemDetails
            {
                Title = Code,
                Detail = Description,
                Status = 400,
            };
        }
    }
}
