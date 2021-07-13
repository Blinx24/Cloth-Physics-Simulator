using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Clase auxiliar Arista para la definición de cada arista
/// de los triángulos de la malla del objeto.
/// </summary>
public class Edge
{
    #region EdgeVertices
    
    /// <summary>
    /// Vértice A de la arista.
    /// </summary>
    public int VertexA;
    /// <summary>
    /// Vértice B de la arista.
    /// </summary>
    public int VertexB;
    /// <summary>
    /// Vértice opuesto.
    /// </summary>
    public int OtherVertex;
    
    #endregion

    /// <summary>
    /// Constructor con parámetros de entrada.
    /// </summary>
    /// <param name="a">Vértice correspondiente a 'A'.</param>
    /// <param name="b">Vértice correspondiente a 'B'.</param>
    /// <param name="0">Vértice correspondiente al opuesto.</param>
    public Edge(int a, int b, int o)
    {
        VertexA = a;
        VertexB = b;
        OtherVertex = o;
    }

    /// <summary>
    /// Devuelve una cadena de caracteres con la clave de la arista.
    /// P.E. Arista(0,1,2) -> Key = 01
    /// </summary>
    public string GetKey()
    {
        return VertexA.ToString() + VertexB.ToString();
    }
}
