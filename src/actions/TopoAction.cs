﻿#region copyright

// Copyright (c) 2021, 2022, 2023 Mark A. Olbert 
// https://www.JumpForJoySoftware.com
// TopoAction.cs
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

using Microsoft.Extensions.Logging;

namespace J4JSoftware.Utilities;

public abstract class TopoAction<TSource> : IAction<TSource>
{
    protected TopoAction(
        ILoggerFactory? loggerFactory
    )
    {
        Logger = loggerFactory?.CreateLogger( GetType() );
    }

    protected ILogger? Logger { get; }

    public bool Process( TSource src )
    {
        if( !Initialize( src ) )
            return false;

        if( !ProcessLoop( src ) )
            return false;

        return Finalize( src );
    }

    // processors are equal if they are the same type, so duplicate instances of the 
    // same type are always equal (and shouldn't be present in the processing set)
    public bool Equals( IAction<TSource>? other )
    {
        if( other == null )
            return false;

        return other.GetType() == GetType();
    }

    bool IAction.Process( object src )
    {
        if( src is TSource castSrc )
            return Process( castSrc );

        Logger?.LogError( "Expected a '{0}' but got a '{1}'", typeof( IAction<TSource> ), src.GetType() );

        return false;
    }

    protected virtual bool Initialize( TSource src ) => true;

    protected virtual bool Finalize( TSource src ) => true;

    protected abstract bool ProcessLoop( TSource src );
}
