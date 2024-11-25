using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
using Get.Data.Collections;
using Get.Data.DataTemplates;
using Get.Data.Helpers;
using Get.Data.Properties;
using Get.Data.XACL;
using Gtudios.UI.TooManyTabs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
namespace TooManyTabs.Test;

class MainWindow : Window
{
    public MainWindow()
    {
        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new MicaBackdrop();
        var tooManyTabsView = new TooManyTabsView<string>()
        {
            Margin = new(0, 30, 0, 0),
            TabHeaderTemplate = new DataTemplate<string, UIElement>(
                    root => new TextBlock()
                    .WithCustomCode(x =>
                        TextBlock.TextProperty.AsProperty<TextBlock, string>(x)
                        .Bind(root, ReadOnlyBindingModes.OneWay)
                    )
                ),
            TabContentTemplate = new DataTemplate<string, UIElement>(
                root => new TextBlock { Margin = new(16) }
                .WithCustomCode(x =>
                    TextBlock.TextProperty.AsProperty<TextBlock, string>(x)
                    .Bind(root.Select(x => $"You are currently looking at {x}"), ReadOnlyBindingModes.OneWay)
                )
            )
        };
        static TooManyTabsSingleItem<string> Create(string Item)
        {
            TooManyTabsSingleItem<string> item = new(Item)
            {
                Name = Item
            };
            item.NameProperty.ValueChanged += (_, _) => Debugger.Break();
            item.ItemProperty.ValueChanged += (_, _) => Debugger.Break();
            return item;
        }
        TooManyTabsMultiItem<string> current = new();
        current.Tabs.Add(Create("Item 1"));
        current.Tabs.Add(Create("Item 2"));
        current.Tabs.Add(
            new TooManyTabsStackItem<string>() { Name = "Stacking Tabs Horizontally", Orientation = Orientation.Horizontal }
            .WithCustomCode(x =>
            {
                x.Tabs.Add(
                    new TooManyTabsMultiItem<string>()
                    .WithCustomCode(x =>
                    {
                        x.Tabs.Add(Create("Item 3"));
                        x.Tabs.Add(Create("Item 4"));
                    })
                );
                x.Tabs.Add(
                    new TooManyTabsMultiItem<string>()
                    .WithCustomCode(x =>
                    {
                        x.Tabs.Add(Create("Item 5"));
                        x.Tabs.Add(Create("Item 6"));
                    })
                );
                x.Tabs.Add(
                    new TooManyTabsMultiItem<string>()
                    .WithCustomCode(x =>
                    {
                        x.Tabs.Add(Create("Item 7"));
                        x.Tabs.Add(Create("Item 8"));
                    })
                );
            })
        );
        current.Tabs.Add(
            new TooManyTabsStackItem<string>() { Name = "Stacking Tabs Vertically", Orientation = Orientation.Vertical }
            .WithCustomCode(x =>
            {
                x.Tabs.Add(
                    new TooManyTabsMultiItem<string>()
                    .WithCustomCode(x =>
                    {
                        x.Tabs.Add(Create("Item 9"));
                        x.Tabs.Add(Create("Item 10"));
                    })
                );
                x.Tabs.Add(
                    new TooManyTabsMultiItem<string>()
                    .WithCustomCode(x =>
                    {
                        x.Tabs.Add(Create("Item 11"));
                        x.Tabs.Add(new TooManyTabsMultiItem<string>()
                        {
                            Name = "Even More Tabs!"
                        }.WithCustomCode(x =>
                        {
                            x.Tabs.Add(Create("Item 12"));
                            x.Tabs.Add(Create("Item 13"));
                            x.Tabs.Add(new TooManyTabsStackItem<string>() { Name = "Stacking Tabs Even More!", Orientation = Orientation.Horizontal }
                                .WithCustomCode(x =>
                                {
                                    x.Tabs.Add(
                                        new TooManyTabsMultiItem<string>()
                                        .WithCustomCode(x =>
                                        {
                                            x.Tabs.Add(Create("Item 14"));
                                            x.Tabs.Add(Create("Item 15"));
                                        })
                                    );
                                    x.Tabs.Add(
                                        new TooManyTabsMultiItem<string>()
                                        .WithCustomCode(x =>
                                        {
                                            x.Tabs.Add(Create("Item 16"));
                                            x.Tabs.Add(Create("Item 17"));
                                        })
                                    );
                                    x.Tabs.Add(
                                        new TooManyTabsMultiItem<string>()
                                        .WithCustomCode(x =>
                                        {
                                            x.Tabs.Add(Create("Item 18"));
                                            x.Tabs.Add(Create("Item 19"));
                                        })
                                    );
                                }));
                        }));
                    })
                );
            })
        );
        tooManyTabsView.TooManyTabsItem = current;
        Content = tooManyTabsView;
    }
}
