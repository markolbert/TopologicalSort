﻿#region copyright

// Copyright (c) 2021, 2022, 2023 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// IAction.cs
//
// This file is part of JumpForJoy Software's TopologicalSort.
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

#endregion

using System;

namespace J4JSoftware.Utilities;

public interface IAction
{
    bool Process( object src );
}

public interface IAction<TSource> : IAction, IEquatable<IAction<TSource>>
{
    bool Process( TSource src );
}
