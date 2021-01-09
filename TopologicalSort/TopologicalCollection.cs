﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace J4JSoftware.Utilities
{
    public class TopologicalCollection<T>
        where T : class, IEquatable<T>
    {
        private readonly IEqualityComparer<T>? _comparer;
        private readonly HashSet<TopologicalNode<T>> _nodes = new HashSet<TopologicalNode<T>>();
        private readonly HashSet<TopologicalDependency<T>> _dependencies = new HashSet<TopologicalDependency<T>>();

        public TopologicalCollection( IEqualityComparer<T>? comparer = null )
        {
            _comparer = comparer;
        }

        public void Clear()
        {
            _nodes.Clear();
            _dependencies.Clear();
        }

        public bool ValuesAreEqual( T x, T y )
        {
            if( _comparer == null )
                return x.Equals(y);

            return _comparer.Equals( x, y );
        }

        public bool NodesAreEqual(TopologicalNode<T> x, TopologicalNode<T> y)
        {
            if( _comparer == null )
                return x.Value.Equals( y.Value );

            return _comparer.Equals(x.Value, y.Value);
        }

        public bool DependenciesAreEqual( TopologicalDependency<T> x, TopologicalDependency<T> y )
        {
            if( _comparer == null )
                return x.AncestorNode.Value.Equals( y.AncestorNode.Value )
                       && x.DependentNode.Value.Equals( y.DependentNode.Value );

            return _comparer.Equals( x.AncestorNode.Value, y.AncestorNode.Value )
                   && _comparer.Equals( x.DependentNode.Value, y.DependentNode.Value );
        }

        public List<TopologicalNode<T>> GetDependents( TopologicalNode<T> ancestor )
        {
            return _dependencies.Where( x => ValuesAreEqual(x.AncestorNode.Value, ancestor.Value) )
                .Select( x => x.DependentNode )
                .Distinct()
                .ToList();
        }

        public List<TopologicalNode<T>> GetAncestors( TopologicalNode<T> dependent )
        {
            return _dependencies.Where( x => ValuesAreEqual( x.DependentNode.Value, dependent.Value ) )
                .Select( x => x.AncestorNode )
                .Distinct()
                .ToList();
        }

        public List<TopologicalNode<T>> GetRoots()
        {
            return _nodes.Where( x => !_dependencies.Any( d => NodesAreEqual( d.AncestorNode, x ) ) )
                .Select( x => x )
                .Distinct()
                .ToList();
        }

        public TopologicalNode<T> AddValue( T value )
        {
            var retVal = _nodes.FirstOrDefault(n => ValuesAreEqual(n.Value, value));

            if (retVal != null)
                return retVal;

            retVal = new TopologicalNode<T>(value, this, _comparer);
            _nodes.Add(retVal);

            return retVal;
        }

        public TopologicalNode<T> AddDependency( T ancestorValue, T dependentValue )
        {
            var ancestor = AddValue( ancestorValue );
            var dependent = AddValue( dependentValue );

            if( ValuesAreEqual( ancestorValue, dependentValue ) )
                return dependent;

            var dependency = new TopologicalDependency<T>(ancestor, dependent, this);

            if( !_dependencies.Any( x => DependenciesAreEqual( x, dependency ) ) )
                _dependencies.Add( dependency );

            return dependent;
        }

        public bool Remove( T toRemove )
        {
            var node = _nodes.FirstOrDefault( x => ValuesAreEqual( x.Value, toRemove ) );

            if( node == null )
                return false;

            var edgesToRemove = new List<TopologicalDependency<T>>();

            foreach( var dependency in _dependencies )
            {
                if( ValuesAreEqual(dependency.AncestorNode.Value, toRemove) 
                    || ValuesAreEqual(dependency.DependentNode.Value, toRemove))
                    edgesToRemove.Add( dependency );
            }

            foreach( var edgeToRemove in edgesToRemove )
            {
                _dependencies.Remove( edgeToRemove );
            }

            return _nodes.Remove(node);
        }

        public bool Sort(out List<T>? sorted, out List<TopologicalDependency<T>>? remainingEdges  )
        {
            sorted = null;
            remainingEdges = null;

            switch( _nodes.Count )
            {
                case 0:
                    return false;

                case 1:
                    if( _dependencies.Count > 0 )
                        return false;

                    break;
            }

            // Empty list that will contain the sorted elements
            var retVal = new Stack<TopologicalNode<T>>();

            // work with a copy of edges so we can keep re-sorting
            var dependencies = new HashSet<TopologicalDependency<T>>( _dependencies.ToArray() );

            // Set of all nodes with no incoming edges
            var noIncomingEdges = new HashSet<TopologicalNode<T>>( _nodes.Where( n =>
                dependencies.All( e => !NodesAreEqual( e.DependentNode, n ) ) ) );

            // while noIncomingEdges is non-empty do
            while (noIncomingEdges.Any())
            {
                //  remove a node from noIncomingEdges
                var nodeToRemove = noIncomingEdges.First();
                noIncomingEdges.Remove(nodeToRemove);

                // add removed node to stack
                retVal.Push(nodeToRemove);

                // for each targetNode with an edge from nodeToRemove to targetNode do
                foreach (var edge in dependencies.Where(e => NodesAreEqual(e.AncestorNode, nodeToRemove) ).ToList())
                {
                    var targetNode = edge.DependentNode;

                    // remove edge from the graph
                    dependencies.Remove(edge);

                    // if targetNode has no other incoming edges then
                    if (dependencies.All(x => !NodesAreEqual(x.DependentNode, targetNode) ))
                    {
                        // insert targetNode into noIncomingEdges
                        noIncomingEdges.Add(targetNode);
                    }
                }
            }

            remainingEdges = dependencies.ToList();

            if ( dependencies.Any() )
                return false;

            var tempSorted = retVal.ToList();

            // for reasons I've never understood the list comes out backwards...
            tempSorted.Reverse();

            sorted = tempSorted.Select( x => x.Value )
                .ToList();

            return true;
        }
    }
}