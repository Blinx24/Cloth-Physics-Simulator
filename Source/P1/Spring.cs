using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Spring
{
    #region NodeVariables
    
    /// <summary>
    /// Nodo A del muelle.
    /// </summary>
    private Node NodeA;
    /// <summary>
    /// Vértice B de la arista.
    /// </summary>
    private Node NodeB;
    
    #endregion
    
    #region SpringVariables
    
    /// <summary>
    /// Longitud actual del muelle.
    /// </summary>
    private float Length;
    /// <summary>
    /// Longitud inicial del muelle (reposo).
    /// </summary>
    private float Length0;
    /// <summary>
    /// Rigidez del muelle.
    /// </summary>
    private float Stiffness;
    /// <summary>
    /// Amortiguamiento del muelle.
    /// </summary>
    private float Damping;
    
    #endregion

    /// <summary>
    /// Constructor del muelle con parámetros.
    /// </summary>
    /// <param name="a">Nodo'A' del muelle.</param>
    /// <param name="b">Nodo 'B' del muelle.</param>
    /// <param name="k">Constante de rigidez del muelle.</param>
    /// <param name="d">Constante de amortiguamiento del muelle.</param>
    public Spring(Node a, Node b, float k, float d)
    {
        NodeA = a;
        NodeB = b;
        UpdateLength();
        Length0 = Length;
        Stiffness = k;
        Damping = d;
    }

    /// <summary>
    /// Actualización de la longitud del muelle.
    /// </summary>
    public void UpdateLength()
    {
        // Cálculo de la distancia
        Length = (NodeB.Pos - NodeA.Pos).magnitude;
    }

    /// <summary>
    /// Cálculo de las fuerzas del muelle al nodo.
    /// </summary>
    public void ComputeForces()
    {
        // Vector director
        Vector3 u = (NodeA.Pos - NodeB.Pos);
        u.Normalize();   
        
        // Fuerza nodo A
        Vector3 fA = - Stiffness * (Length - Length0) * u;  // Si se define u de B a A, debería ser +

        // Suma a las fuerzas la fuerza elástica
        NodeA.Force += fA;
        NodeB.Force -= fA;

        // Suma a las fuerzas la fuerza de amortiguamiento
        NodeA.Force -= Damping * (u * (NodeA.Vel - NodeB.Vel).magnitude);
        NodeB.Force += Damping * (u * (NodeA.Vel - NodeB.Vel).magnitude);
    }
}
