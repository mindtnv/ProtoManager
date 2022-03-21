using ProtoManager.Abstractions;
using PuppeteerSharp;
using SmsActivator.Abstractions;

namespace ProtoManager;

public class ProtonMailRegistrationService : IProtonMailRegistrationService
{
    private readonly IBrowserProvider _browserProvider;
    private readonly ISmsActivator _smsActivator;

    public ProtonMailRegistrationService(ISmsActivator smsActivator,
        IBrowserProvider browserProvider)
    {
        _smsActivator = smsActivator;
        _browserProvider = browserProvider;
    }

    public async Task<ProtonMailRegistrationResult> RegisterAsync(ProtonMailRegistrationOptions options,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(options.Username))
            throw new ArgumentNullException(nameof(options.Username));
        if (string.IsNullOrEmpty(options.Password))
            throw new ArgumentNullException(nameof(options.Password));

        var browser = _browserProvider.GetBrowser();
        var account = new ProtonMailAccount
        {
            Email = options.Username,
            Password = options.Password,
        };
        try
        {
            var page = await browser.NewPageAsync();
            await OpenSignupForm(page);
            await FillAndSubmitSignupForm(page, options.Username, options.Password);
            await using var manager = await _smsActivator.ActivateAsync(new ActivationParameters
            {
                Service = "Protonmail",
            });
            account.Phone = manager.Phone;
            await OpenSmsVerificationFrom(page, manager);
            var code = await manager.GetCodeAsync(cancellationToken);
            await SubmitSmsVerificationForm(page, code);
            await ApplySettings(page);
            return new ProtonMailRegistrationResult
            {
                IsOk = true,
                Account = account,
            };
        }
        catch (Exception)
        {
            return new ProtonMailRegistrationResult
            {
                IsOk = false,
                Account = account,
            };
        }
    }

    private async Task OpenSignupForm(Page page)
    {
        await page.GoToAsync("https://account.protonmail.com/signup?language=ru");
        await page.WaitForSelectorAsync(".button-large[type=submit]", new WaitForSelectorOptions
        {
            Visible = true,
        });
    }

    private async Task FillAndSubmitSignupForm(Page page, string email, string password)
    {
        await page.FocusAsync("input#username");
        await page.Keyboard.SendCharacterAsync(email);
        await page.FocusAsync("input#password");
        await page.Keyboard.SendCharacterAsync(password);
        await page.FocusAsync("input#repeat-password");
        await page.Keyboard.SendCharacterAsync(password);
        await page.ClickAsync("button[type=submit]");
        await page.WaitForTimeoutAsync(1000);
        await page.WaitForSelectorAsync(".full-loader", new WaitForSelectorOptions
        {
            Visible = true,
        });
        await page.WaitForSelectorAsync(".full-loader", new WaitForSelectorOptions
        {
            Hidden = true,
        });
        await page.ClickAsync("button[data-testid*=телеф]");
        await page.WaitForSelectorAsync("#recovery-phone", new WaitForSelectorOptions
        {
            Visible = true,
        });
    }

    private async Task OpenSmsVerificationFrom(Page page, IActivationManager manager)
    {
        await page.FocusAsync("#recovery-phone");
        await page.Keyboard.SendCharacterAsync($"+{manager.Phone}");
        await page.ClickAsync("button[type=submit]");
        await page.WaitForSelectorAsync("button[aria-describedby=desc_Free]", new WaitForSelectorOptions
        {
            Visible = true,
        });
        await page.ClickAsync("button[aria-describedby=desc_Free]");
        await page.WaitForSelectorAsync("button[data-testid*=телеф]", new WaitForSelectorOptions
        {
            Visible = true,
        });
        await page.ClickAsync("button[data-testid*=телеф]");
        await page.WaitForSelectorAsync("button.button-large", new WaitForSelectorOptions
        {
            Visible = true,
        });
        await page.ClickAsync("button.button-large");
        await page.WaitForSelectorAsync("#verification", new WaitForSelectorOptions
        {
            Visible = true,
        });
        await page.FocusAsync("#verification");
    }

    private async Task SubmitSmsVerificationForm(Page page, string code)
    {
        await page.Keyboard.SendCharacterAsync(code);
        await page.ClickAsync("button.button-large");
        await page.WaitForSelectorAsync("button.button-large[type=submit]", new WaitForSelectorOptions
        {
            Visible = true,
        });
    }

    private async Task ApplySettings(Page page)
    {
        await page.ClickAsync("button.button-large[type=submit]");
        await page.WaitForTimeoutAsync(150);
        await page.ClickAsync("button.button-large[type=submit]");
        await page.WaitForTimeoutAsync(150);
        await page.ClickAsync("button.button-large[type=submit]");
        await page.WaitForTimeoutAsync(150);
        await page.ClickAsync("button.button-large[type=submit]");
        await page.WaitForTimeoutAsync(150);
    }
}