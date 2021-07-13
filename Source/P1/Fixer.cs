using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase auxiliar 'Fixer' para enganchar la tela a un objeto.
/// </summary>
public class Fixer : MonoBehaviour
{
    #region ClothVars
    
    /// <summary>
    /// Referencia a la tela del escenario.
    /// </summary>
    private MassSpring Cloth;
    /// <summary>
    /// Lista de nodos fijos de la tela.
    /// </summary>
    private List<Node> FixedNodes;
    /// <summary>
    /// Variable controladora para indicar si se encuentra dentro del objeto 'Fixer' o no.
    /// </summary>
    private bool IsInside;
    
    #endregion
    
    #region FixerParams
    /// <summary>
    /// Límites del objeto 'Fixer'.
    /// </summary>
    private Bounds Bounds;
    /// <summary>
    /// Posición inicial del objeto 'Fixer'.
    /// </summary>
    private Vector3 InitPos;
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        // Inicialización de parámetros
        Cloth = GameObject.Find("Cloth").GetComponent<MassSpring>();
        Bounds = GetComponent<Collider>().bounds;
        IsInside = false;
        FixedNodes = new List<Node>();
        InitPos = transform.position;

        // Por cada nodo en la lista de nodos de la tela
        foreach (Node node in Cloth.Nodes)
        {
            // Comprobación de la posición del nodo
            IsInside = Bounds.Contains(node.Pos);

            // Si se encuentra dentro del objeto 'Fixer'
            if (IsInside)
            {
                // Inserción nodo a la lista de fijos
                FixedNodes.Add(node);
                // Actualización de parámetro
                node.Fixed = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Vector de movimiento del objeto
        Vector3 movement = transform.position - InitPos; 
        // Actualización posición de nodos fijos en función del movimiento
        for (int i = 0; i < FixedNodes.Count; i++)
        {
            FixedNodes[i].Pos += movement;
        }
        // Actualización de la posición
        InitPos = transform.position;
    }
}
