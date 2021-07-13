using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{

    #region NodeVariables
    /// <summary>
    /// Posición del nodo.
    /// </summary>
    public Vector3 Pos;
    /// <summary>
    /// Velocidad del nodo.
    /// </summary>
    public Vector3 Vel;
    /// <summary>
    /// Fuerza del nodo.
    /// </summary>
    public Vector3 Force;
    /// <summary>
    /// Masa del nodo.
    /// </summary>
    public float Mass;
    /// <summary>
    /// ¿Se puede mover el nodo?.
    /// </summary>
    public bool Fixed;
    /// <summary>
    /// Coeficiente de amortiguamiento del nodo.
    /// </summary>
    public float Damping;
    
    #endregion
    
    #region NodeVariables
    
    /// <summary>
    /// Referencia al objeto MassSpring.
    /// </summary>
    public MassSpring Manager;
    
    #endregion

    /// <summary>
    /// Constructor con parámetros del nodo.
    /// </summary>
    /// <param name="_pos">Posición inicial del nodo.</param>
    /// <param name="_mass">Masa del nodo.</param>
    /// <param name="_damping">Constante de amortiguamiento del nodo.</param>
    /// <param name="_manager">Referencia al manejador 'MassSpring'.</param>
    public Node(Vector3 _pos, float _mass, float _damping, MassSpring _manager)
    {
        Pos = _pos;
        Vel = Vector3.zero;
        Force = Vector3.zero;
        Mass = _mass;
        Damping = _damping;
        Fixed = false;
        Manager = _manager;
    }

    /// <summary>
    /// Cálculo de fuerzas.
    /// </summary>
    public void ComputeForces()
    {
        // Cálculo gravedad
        this.Force += this.Mass * Manager.Gravity;
        
        // Cálculo de amortiguamiento
        this.Force -= this.Damping * this.Vel;
    }
}
