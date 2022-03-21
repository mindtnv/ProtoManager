using ProtoManager.Abstractions;
using PuppeteerSharp;

namespace ProtoManager;

public class ProtonMailManager : IProtonMailManager
{
    private readonly Dictionary<string, Mail> _mails = new();
    private readonly Page _page;
    private bool _scriptInitialized;

    public ProtonMailManager(Page page)
    {
        _page = page;
    }

    public async Task<Mail> GetMailAsync(string id)
    {
        if (!_scriptInitialized)
            await InitializeScripts();

        if (!_mails.ContainsKey(id))
        {
            var mail = await _page.EvaluateExpressionAsync<MailResponse>(
                "(async () => { " +
                $"let response = await window.messageApi.get(\"{id}\");" +
                "return { mail : response.data.Message, body: (await (new window.MessageModel(response.data.Message)).decryptBody()).message} })()"
            );
            mail.Mail.Body = mail.Body;
            _mails[id] = mail.Mail;
        }

        return _mails[id];
    }

    public async Task<IEnumerable<MailInfo>> GetMailInfosAsync(int page)
    {
        if (!_scriptInitialized)
            await InitializeScripts();

        var result = await _page.EvaluateExpressionAsync<MailInfo[]>(
            $"window.messageApi.query({{LabelID: 5,Limit: 100,Page: {page}}}).then(response => {{return response.data.Messages}})"
        );
        return result;
    }

    public ValueTask DisposeAsync() => _page.DisposeAsync();

    private async Task InitializeScripts()
    {
        await _page.EvaluateExpressionAsync(
            "window.conversationApi = window.angular.element(document.body).injector().get('conversationApi');" +
            "window.labelsModel = window.angular.element(document.body).injector().get('labelsModel');" +
            "window.labelModel = window.angular.element(document.body).injector().get('Label');" +
            "window.MessageModel = window.angular.element(document.body).injector().get('messageModel');" +
            "window.addressesModel = window.angular.element(document.body).injector().get('addressesModel');" +
            "window.messageApi = window.angular.element(document.body).injector().get('messageApi');" +
            "window.encryptMessage = window.angular.element(document.body).injector().get('encryptMessage');" +
            "console.log('script initialized');");
        _scriptInitialized = true;
    }

    private class MailResponse
    {
        public Mail Mail { get; set; }
        public string Body { get; set; }
    }
}