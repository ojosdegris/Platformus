﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Platformus.Barebone;
using Platformus.Globalization.Frontend.ViewModels;
using Platformus.Navigation.Data.Abstractions;
using Platformus.Navigation.Data.Models;

namespace Platformus.Navigation.Frontend.ViewModels.Shared
{
  public class MenuItemViewModelBuilder : ViewModelBuilderBase
  {
    public MenuItemViewModelBuilder(IHandler handler)
      : base(handler)
    {
    }

    public MenuItemViewModel Build(MenuItem menuItem)
    {
      return new MenuItemViewModel()
      {
        Name = this.GetObjectLocalizationValue(menuItem.NameId),
        Url = menuItem.Url,
        MenuItems = this.handler.Storage.GetRepository<IMenuItemRepository>().FilteredByMenuItemId(menuItem.Id).Select(
          mi => new MenuItemViewModelBuilder(this.handler).Build(mi)
        )
      };
    }

    public MenuItemViewModel Build(CachedMenuItem cachedMenuItem)
    {
      IEnumerable<CachedMenuItem> cachedMenuItems = new CachedMenuItem[] { };

      if (!string.IsNullOrEmpty(cachedMenuItem.CachedMenuItems))
        cachedMenuItems = JsonConvert.DeserializeObject<IEnumerable<CachedMenuItem>>(cachedMenuItem.CachedMenuItems);

      return new MenuItemViewModel()
      {
        Name = cachedMenuItem.Name,
        Url = cachedMenuItem.Url,
        MenuItems = cachedMenuItems.OrderBy(cmi => cmi.Position).Select(
          cmi => new MenuItemViewModelBuilder(this.handler).Build(cmi)
        )
      };
    }
  }
}