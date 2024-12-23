using System.Threading.Tasks;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using Get.Data.Properties;
using Get.Data.UIModels;
using Gtudios.UI.Controls.Tabs;
using Gtudios.UI.Windowing;
namespace Gtudios.UI.TooManyTabs.AppModel;
abstract partial class TooManyTabsAppModel<T>
{
    public virtual bool AllowTabDropNewWindowCreation { get; } = true;
    public virtual bool CloseWindowOnNoTabs { get; } = true;
    protected abstract Task<T> CreateItemOverrideAsync(Window w);
    protected abstract void OnTabClosing(object? sender, TabClosingRequestEventArgs<TooManyTabsItem<T>> e);
    protected abstract IDataTemplate<T, UIElement> TabContentTemplate { get; }
    protected abstract IDataTemplate<ITooManyTabsStyle<T>, TabItem<TooManyTabsItem<T>>> TabStyleTemplate { get; }
    public virtual IDataTemplate<TooManyTabsMultiItem<T>, UIElement>? TabViewHeader { get; } = default;
    protected virtual IDataTemplate<T, UIElement>? ToolbarTemplate { get; } = default;
    protected abstract ITooManyTabsStyle<T> GetStyleForSingleItem(T value);
    protected abstract ITooManyTabsStyle<T> CreateStyleForMultiItem();
    protected abstract ITooManyTabsStyle<T> CreateStyleForStackItem();
    protected virtual async Task<Window> CreateWindowOverrideAsync()
    {
        var window = await Window.CreateAsync();
#if WINDOWS_UWP
        // apply backdrop material to window.RootContent
        var page = new Page();
        window.RootContent = page;
        //BackdropMaterial.SetApplyToRootOrPageBackground(page, true);
#else
        if (window is WindowXAMLWindow xamlWindow)
        {
            //xamlWindow.ExtendsContentIntoTitleBar = true;
            xamlWindow.PlatformWindow.SystemBackdrop = new MicaBackdrop();
        }
#endif
        return window;
    }
    protected virtual TooManyTabsView<T> CreateTooManyTabsOverride(Window window)
    {
        return new(new(CreateStyleForMultiItem()));
    }
    protected virtual void CloseWindow(Window window)
    {
        window.Close();
    }
}