using System.Threading.Tasks;
using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.DataTemplates;
using Get.Data.Properties;
using Get.Data.UIModels;
using Gtudios.UI.Controls.Tabs;
using Gtudios.UI.Windowing;
namespace Gtudios.UI.TooManyTabs.AppModel;
public partial class TooManyTabsAppModel<T>
{
    Dictionary<nint, TooManyTabsWindow<T>> WindowsMap { get; } = [];
    public TooManyTabsWindow<T> GetForWindow(Window window) => WindowsMap[window.WindowHandle];
    Dictionary<T, TooManyTabsSingleItem<T>> ItemMap { get; } = [];
    public TooManyTabsSingleItem<T> this[T item]
    {
        get => ItemMap[item];
        private set => ItemMap[item] = value;
    }
}
