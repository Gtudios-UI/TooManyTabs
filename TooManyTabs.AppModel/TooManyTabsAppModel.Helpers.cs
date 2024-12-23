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
    async Task<TooManyTabsSingleItem<T>> CreateSingleTabAsync(Window w)
    {
        var item = await CreateItemOverrideAsync(w);
        var tmti = new TooManyTabsSingleItem<T>(item, GetStyleForSingleItem(item));
        this[tmti.Item] = tmti;
        return tmti;
    }
}
