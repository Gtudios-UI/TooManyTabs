using Get.UI.Data;
using Get.Data.Properties;
using Get.Data.DataTemplates;
using Get.Data.Bindings;
using Get.Data.Helpers;
using Get.UI.Controls.Panels;
using Get.Data.Collections.Conversion;
namespace Gtudios.UI.TooManyTabs;
class TooManyTabsStackView<T> : TemplateControl<OrientedStack>
{
    public Property<TooManyTabsStackItem<T>> TooManyTabsMultiItemProperty { get; }
    TooManyTabsView<T> _Parent;
    public TooManyTabsStackView(TooManyTabsView<T> Parent, TooManyTabsStackItem<T> initial)
    {
        _Parent = Parent;
        TooManyTabsMultiItemProperty = new(initial);
    }
    readonly ReadOnlyProperty<int> NegativeOne = new(-1);
    IDisposable? disposable;
    protected override void Initialize(OrientedStack rootElement)
    {
        void Set(TooManyTabsStackItem<T> newItem)
        {
            disposable?.Dispose();
            disposable = newItem.Tabs.Bind(
                rootElement.Children.AsGDCollection(),
                new DataTemplate<TooManyTabsItem<T>, UIElement>(
                    root =>
                        new TooManyTabsViewSelector<T>(_Parent, root)
                        .WithCustomCode(x => OrientedStack.LengthProperty.SetValue(x, new(1, GridUnitType.Star)))
                )
            );
            rootElement.OrientationProperty.Bind(newItem.OrientationProperty, ReadOnlyBindingModes.OneWay);
        }
        Set(TooManyTabsMultiItemProperty.Value);
        TooManyTabsMultiItemProperty.ValueChanged += (_, x) => Set(x);
        
    }
}