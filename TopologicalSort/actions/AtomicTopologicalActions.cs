﻿using System.Collections.Generic;
using J4JSoftware.Logging;

namespace J4JSoftware.Utilities
{
    public class AtomicTopologicalActions<TSymbol> : TopologicalCollection<IEnumerableProcessor<TSymbol>>, IProcessorCollection<TSymbol>
    {
        protected AtomicTopologicalActions( 
            ExecutionContextBase context,
            IJ4JLogger logger 
            )
        {
            Context = context;

            Logger = logger;
            Logger.SetLoggedType( this.GetType() );
        }

        protected IJ4JLogger Logger { get; }
        protected ExecutionContextBase Context { get; }

        // symbols must be able to reset so it can be iterated multiple times
        public virtual bool Process( IEnumerable<TSymbol> symbols )
        {
            if( !Initialize( symbols ) )
                return false;

            var allOkay = true;

            if( !Sort( out var procesorNodes, out var remainingEdges ) )
            {
                Logger.Error( "Couldn't topologically sort processors" );
                return false;
            }

            procesorNodes!.Reverse();

            foreach( var node in procesorNodes! )
            {
                allOkay &= node.Process( symbols );

                if( !allOkay && Context.StopOnFirstError )
                    break;
            }

            return allOkay && Finalize( symbols );
        }

        protected virtual bool Initialize( IEnumerable<TSymbol> symbols ) => true;
        protected virtual bool Finalize( IEnumerable<TSymbol> symbols ) => true;
    }
}