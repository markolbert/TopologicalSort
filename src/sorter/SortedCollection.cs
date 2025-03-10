﻿#region copyright

// Copyright (c) 2021, 2022, 2023 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// SortedCollection.cs
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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace J4JSoftware.Utilities;

public abstract class SortedCollection<T> : ISortedCollection<T>
    where T : class, ISortable<T>
{
    private readonly List<int> _activatedIndices = [];
    private readonly List<T> _items = [];

    private List<T>? _sorted;

    protected SortedCollection(
        ILoggerFactory? loggerFactory = null
    )
    {
        Logger = loggerFactory?.CreateLogger( GetType() );
    }

    protected ILogger? Logger { get; }
    protected List<T> Available { get; } = [];

    public List<T> SortedSequence
    {
        get
        {
            if( _sorted != null )
                return _sorted;

            _sorted = SortItems();

            return _sorted;
        }
    }

    protected abstract bool SetPredecessors();

    public void Add( T item )
    {
        _items.Add( item );
        _sorted = null;
    }

    public void AddRange( IEnumerable<T> items )
    {
        _items.AddRange( items );
        _sorted = null;
    }

    private List<T> SortItems()
    {
        var retVal = new List<T>();

        Available.Clear();
        Available.AddRange( _items );

        if( !SetPredecessors() )
            return retVal;

        // remove the items that were activated. there should
        // be only one item left (the root item)
        foreach( var idx in _activatedIndices.OrderByDescending( x => x ) )
        {
            Available.RemoveAt( idx );
        }

        switch( Available.Count )
        {
            case 0:
                Logger?.LogError( "No root {type} defined", typeof( T ) );
                break;

            case 1:
                // should already be null, but just in case...
                Available[ 0 ].Predecessor = null;

                _items.Add( Available[ 0 ] );

                if( TopologicalSorter.Sort( _items, out var result ) )
                    SortedSequence.AddRange( result! );
                else
                    Logger?.LogError( "Couldn't create execution sequence for {type}", typeof( T ) );

                break;

            default:
                Logger?.LogError( "Multiple root {type} objects defined", typeof( T ) );
                break;
        }

        return retVal;
    }

    protected bool SetPredecessor<TNode, TPredecessorNode>()
        where TNode : T
        where TPredecessorNode : T
    {
        var nodeType = typeof( TNode );
        var predecessorType = typeof( TPredecessorNode );

        var selectedIdx = Available.FindIndex( x => x.GetType() == nodeType );

        if( selectedIdx < 0 )
        {
            Logger?.LogError( "Couldn't find '{nodeType}'", nodeType );
            return false;
        }

        var predecessor = Available.FirstOrDefault( x => x.GetType() == predecessorType );

        if( predecessor == null )
        {
            Logger?.LogError( "Couldn't find '{predecessorType}'", predecessorType );
            return false;
        }

        Available[ selectedIdx ].Predecessor = predecessor;

        _items.Add( Available[ selectedIdx ] );
        _activatedIndices.Add( selectedIdx );

        return true;
    }
}
