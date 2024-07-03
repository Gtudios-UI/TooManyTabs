using Get.Data.Collections.Update;
using Get.Data.Collections;
using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
public class TooManyTabsStackItem<T> : TooManyTabsItem<T>
{
    public Property<int> SelectedIndexProperty { get; } = new(0);
    public IUpdateCollection<TooManyTabsItem<T>> Tabs { get; } = new UpdateCollection<TooManyTabsItem<T>>();
    public Property<Orientation> OrientationProperty { get; } = new(Orientation.Horizontal);
    public Orientation Orientation { get => OrientationProperty.Value; set => OrientationProperty.Value = value; }
}