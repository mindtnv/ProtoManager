using ProtoManager.Abstractions;
using PuppeteerSharp;

namespace ProtoManager;

public class ProtonMailLoginService : IProtonMailLoginService
{
    private readonly IBrowserProvider _browserProvider;

    public ProtonMailLoginService(IBrowserProvider browserProvider)
    {
        _browserProvider = browserProvider;
    }

    public async Task<IProtonMailManager> LoginAsync(ProtonMailLoginOptions options,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(options.Username))
            throw new ArgumentNullException(nameof(options.Username));
        if (string.IsNullOrEmpty(options.Password))
            throw new ArgumentNullException(nameof(options.Password));

        var browser = _browserProvider.GetBrowser();
        var page = await browser.NewPageAsync();
        await OpenLoginForm(page);
        await SubmitLoginForm(page, options.Username, options.Password);
        return new ProtonMailManager(page);
    }

    private async Task OpenLoginForm(Page page)
    {
        await page.GoToAsync("https://old.protonmail.com/login");
        await page.WaitForSelectorAsync("#login_btn", new WaitForSelectorOptions
        {
            Visible = true,
        });
        await page.ClickAsync("#login_btn");
    }

    private async Task SubmitLoginForm(Page page, string username, string password)
    {
        await page.FocusAsync("#username");
        await page.Keyboard.SendCharacterAsync(username);
        await page.FocusAsync("#password");
        await page.Keyboard.SendCharacterAsync(password);
        await page.ClickAsync("#login_btn");
        await page.WaitForSelectorAsync("#ptSidebar", new WaitForSelectorOptions
        {
            Visible = true,
        });
        // await Task.WhenAny(
        //     page.WaitForSelectorAsync("#ptSidebar", new WaitForSelectorOptions
        //     {
        //         Visible = true,
        //     }),
        //     page.WaitForSelectorAsync(".proton-notification-template.notification-danger", new WaitForSelectorOptions
        //         {
        //             Visible = true,
        //         })
        //         .ContinueWith(_ => throw new ProtonMailLoginException())
        // );
    }
}