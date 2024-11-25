using Get.Data.Collections.Update;
using Get.Data.Collections;
using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsStackItem<T> : TooManyTabsItem<T>
{
    public IProperty<int> SelectedIndexProperty { get; } = Auto(0);
    public IProperty<Orientation> OrientationProperty { get; } = Auto(Orientation.Horizontal);
    public IUpdateCollection<TooManyTabsItem<T>> Tabs { get; } = new UpdateCollection<TooManyTabsItem<T>>();
}