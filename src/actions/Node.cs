#region copyright

// Copyright (c) 2021, 2022, 2023 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// Node.cs
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
using System.Collections.Generic;

namespace J4JSoftware.Utilities;

public class Node<T>(
    T value,
    Nodes<T> collection,
    IEqualityComparer<T>? comparer = null
)
    where T : class, IEquatable<T>
{
    public T Value { get; } = value;
    public Nodes<T> Collection { get; } = collection;
    public List<Node<T>> Dependents => Collection.GetDependents( this );
    public List<Node<T>> Ancestors => Collection.GetAncestors( this );

    public bool Equals( Node<T>? other )
    {
        if( other == null )
            return false;

        if( comparer == null )
            return other.Value.Equals( Value );

        return comparer.Equals( Value, other.Value );
    }
}
