using Get.Data.DataTemplates;
using Get.UI.Data;
using Get.Data.Bindings.Linq;
using Gtudios.UI.Controls.Tabs;
using Gtudios.UI.TooManyTabs;
using Gtudios.UI.TooManyTabs.AppModel;
using Gtudios.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Get.Data.Bundles;
using static Get.Data.XACL.QuickBindingExtension;
namespace TooManyTabs.AppModel.Test;

class TestApp : TooManyTabsAppModel<string>
{
    int i = 1;
    protected override Task<string> CreateItemOverrideAsync()
    {
        return Task.FromResult($"Item {i++}");
    }
    protected override ITooManyTabsStyle<string> GetStyleForSingleItem(string value)
        => new Style(value);
    protected override ITooManyTabsStyle<string> CreateStyleForMultiItem()
        => new Style("Tab Group");
    protected override ITooManyTabsStyle<string> CreateStyleForStackItem()
        => new Style("Tab Stack");
    protected override IDataTemplate<ITooManyTabsStyle<string>, TabItem<TooManyTabsItem<string>>> TabStyleTemplate
    { get; } = new DataTemplate<ITooManyTabsStyle<string>, TabItem<TooManyTabsItem<string>>>(root => 
        new TabItem<TooManyTabsItem<string>>
        {
            ContentBundle = new ContentBundle<string, UIElement>(((Style)root.CurrentValue).Text)
            {
                ContentBinding = OneWay(from x in root select ((Style)x).Text),
                ContentTemplate = DataTemplates.TextBlockUIElement<string>()
            }
        }
    );
    protected override string GetName(string item)
    {
        return item;
    }
    protected override void OnTabClosing(object sender, TabClosingRequestEventArgs<TooManyTabsItem<string>> e)
    {
        e.RemoveRequest = true;
    }
    protected override async Task<Gtudios.UI.Windowing.Window> CreateWindowOverrideAsync()
    {
        var wnd = await base.CreateWindowOverrideAsync();
        if (wnd is WindowAppWindow w) w.AppWindow.Title = "Too Many Tabs Demo";
        return wnd;
    }
    protected override IDataTemplate<string, UIElement> TabContentTemplate { get; } = DataTemplates.TextBlockUIElement<string>();
}
readonly record struct Style(string Text) : ITooManyTabsStyle<string>;