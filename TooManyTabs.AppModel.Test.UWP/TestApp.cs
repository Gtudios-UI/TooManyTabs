using Get.Data.DataTemplates;
using Get.UI.Data;
using Gtudios.UI.Controls.Tabs;
using Gtudios.UI.TooManyTabs;
using Gtudios.UI.TooManyTabs.AppModel;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml;
namespace TooManyTabs.AppModel.Test.UWP;

class TestApp : TooManyTabsAppModel<string>
{
    int i = 1;
    protected override Task<string> CreateItemOverrideAsync()
    {
        return Task.FromResult($"Item {i++}");
    }
    protected override string GetName(string item)
    {
        return item;
    }
    protected override void OnTabClosing(TooManyTabsItem<string> item, TabClosingHandledEventArgs e)
    {
        // ask it to remove
        e.RemoveRequest = true;
    }
    protected override IDataTemplate<string, UIElement> TabContentTemplate { get; } = DataTemplates.TextBlockUIElement<string>();
}
