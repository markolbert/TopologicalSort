﻿// Copyright (c) 2021, 2022 Mark A. Olbert 
// 
// This file is part of TopologicalSort.
//
// TopologicalSort is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// TopologicalSort is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with TopologicalSort. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace J4JSoftware.Utilities;

public interface ITopologicalSortFactory
{
    bool CreateSortedList<T>( IEnumerable<T> toSort, out List<T>? result )
        where T : class, IEquatable<T>;
}