using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesPizza.Pages;

public class AmogusModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public AmogusModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }
    
    public void OnGet()
    {
        
    }
}