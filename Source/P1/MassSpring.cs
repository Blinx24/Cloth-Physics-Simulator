using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Basic physics manager capable of simulating a given ISimulable
/// implementation using diverse integration methods: explicit,
/// implicit, Verlet and semi-implicit.
/// </summary>
public class MassSpring : MonoBehaviour 
{
	/// <summary>
	/// Default constructor. Zero all. 
	/// </summary>
	public MassSpring()
	{
		this.Paused = true;
		this.TimeStep = 0.02f;
		this.Substeps = 5;
		this.Gravity = new Vector3 (0.0f, -9.81f, 0.0f);
		this.IntegrationMethod = Integration.Explicit;
		this.NodeMass = 1.0f;
		this.TractionStiffness = 500.0f;
		this.BendingStiffness = 250.0f;
		this.Damping = 0.5f;
	}

	/// <summary>
	/// Integration method.
	///		0 = Explícito
	///		1 = Simpléctico
	/// </summary>
	public enum Integration
	{
		Explicit = 0,
		Symplectic = 1,
	};

	#region InEditorVariables

	//************************* PARÁMETROS DE LA SIMULACIÓN *************************//
	/// <summary>
	/// Variable auxiliar para controlar la simulación.
	/// </summary>
	public bool Paused;
	/// <summary>
	/// Tiempo de paso de la simulación.
	/// </summary>
	public float TimeStep;
	/// <summary>
	/// Subpasos de la simulación.
	/// </summary>
	public int Substeps;
	/// <summary>
	/// Gravedad.
	/// </summary>
    public Vector3 Gravity;
	/// <summary>
	/// Método de integración.
	/// </summary>
	public Integration IntegrationMethod;
	/// <summary>
	/// Masa de los nodos del objeto.
	/// </summary>
	public float NodeMass;
	/// <summary>
	/// Rigidez de los muelles de tracción.
	/// </summary>
	public float TractionStiffness;
	/// <summary>
	/// Rigidez de los muelles de flexión.
	/// </summary>
	public float BendingStiffness;
	/// <summary>
	/// Amortiguamiento.
	/// </summary>
	public float Damping;
	//public float WindFriction;

	//************************* LISTAS DE ELEMENTOS DE SIMULACIÓN *************************//
	/// <summary>
	/// Lista de nodos del objeto.
	/// </summary>
	public List<Node> Nodes;
	/// <summary>
	/// Lista de muelles en el objeto.
	/// </summary>
	public List<Spring> Springs;
	/// <summary>
	/// Diccionario de aristas de la malla del objeto.
	/// </summary>
	public Dictionary<string,Edge> Edges;

    #endregion

    #region OtherVariables
	//************************* INFORMACIÓN DEL OBJETO *************************//
    /// <summary>
    /// Malla del objeto.
    /// </summary>
    private Mesh ObjectMesh;
    /// <summary>
    /// Array de vértices de la malla.
    /// </summary>
    private Vector3[] Vertices;
    /// <summary>
    /// Array de triángulos de la malla.
    /// </summary>
    private int[] Triangles;
    
    #endregion

    #region MonoBehaviour

    public void Awake()
    {
	    // INICIALIZACIÓN DE PARÁMETROS //
	    // Ajuste del paso de tiempo en función de los subpasos
	    TimeStep /= Substeps;
	    // Malla
	    ObjectMesh = this.GetComponent<MeshFilter>().mesh;
	    Vertices = ObjectMesh.vertices;						
	    Triangles = ObjectMesh.triangles;
	    // Listas de elementos
	    Nodes = new List<Node>();
	    Springs = new List<Spring>();
	    Edges = new Dictionary<string, Edge>();

	    // OBTENCIÓN DE DATOS //
	    // Nodos
	    foreach (Vector3 vertex in Vertices)
	    {
		    // Conversión a coordenadas globales
		    Vector3 pos = transform.TransformPoint(vertex);
		    // Inserción en lista de nodos
		    Nodes.Add(new Node(pos, NodeMass, Damping, this));
	    }

	    // Muelles
	    int totalTriangles = Triangles.Length / 3;
	    for (int i = 0; i < totalTriangles; i++)
	    {
		    // VARIABLES AUXILIARES //
		    // Vértices triángulos
		    int v0 = Triangles[3 * i + 0];	// Vértice 0
		    int v1 = Triangles[3 * i + 1];	// Vértice 1
		    int v2 = Triangles[3 * i + 2];	// Vértice 2
		    int vAux = 0;	// Vértice auxiliar para ordenación de vértices
		    // Aristas auxiliares
		    Edge auxEdge;
		    Edge auxEdge2;
		    Edge auxEdge3;

		    // GENERACIÓN DE ARISTAS Y MUELLES //
		    // Arista (v0,v1)
		    // Comparación de vértices - Si v0 es mayor, se intercambian sus valores
		    if (v0 > v1)
		    {
			    vAux = v0;
			    v0 = v1;
			    v1 = vAux;
		    }

		    // Generación de arista auxiliar
		    auxEdge = new Edge(v0, v1, v2);
		    // Si la arista ya existe en el diccionario (arista duplicada)
		    if (Edges.ContainsKey(auxEdge.GetKey()))
		    {
			    // Generación de muelle de flexión
			    Springs.Add(new Spring(Nodes[v2], Nodes[Edges[auxEdge.GetKey()].OtherVertex], BendingStiffness, Damping));
		    }
		    // No existía en el diccionario
		    else
		    {
			    // Inserción en diccionario
			    Edges.Add(auxEdge.GetKey(),auxEdge);
			    // Generación de muelle de tracción
			    Springs.Add(new Spring(Nodes[v0], Nodes[v1], TractionStiffness, Damping));
		    }
		    
		    // Arista (v0,v2) (= procedimiento)
		    if (v0 > v2)
		    {
			    vAux = v0;
			    v0 = v2;
			    v2 = vAux;
		    }

		    auxEdge2 = new Edge(v0, v2, v1);
		    if (Edges.ContainsKey(auxEdge2.GetKey()))
		    {
			    Springs.Add(new Spring(Nodes[v1], Nodes[Edges[auxEdge2.GetKey()].OtherVertex], BendingStiffness, Damping));
		    }
		    else
		    {
			    Edges.Add(auxEdge2.GetKey(),auxEdge2);
			    Springs.Add(new Spring(Nodes[v0], Nodes[v2], TractionStiffness, Damping));
		    }
		    
		    // Arista (v1,v2) (= procedimiento)
		    if (v1 > v2)
		    {
			    vAux = v1;
			    v1 = v2;
			    v2 = vAux;
		    }

		    auxEdge3 = new Edge(v1, v2, v0);
		    if (Edges.ContainsKey(auxEdge3.GetKey()))
		    {
			    Springs.Add(new Spring(Nodes[v0], Nodes[Edges[auxEdge3.GetKey()].OtherVertex], BendingStiffness, Damping));
		    }
		    else
		    {
			    Edges.Add(auxEdge3.GetKey(),auxEdge3);
			    Springs.Add(new Spring(Nodes[v1], Nodes[v2], TractionStiffness, Damping));
		    }
	    }
    }

	public void Update()
	{
		if (Input.GetKeyUp (KeyCode.P))
			this.Paused = !this.Paused;
		
		// Transformación de coordenadas globales a locales de vértices
		for (int i = 0; i < Nodes.Count; i++)
		{
			Vector3 pos = Nodes[i].Pos;
			Vertices[i] = transform.InverseTransformPoint(pos);
		}
		ObjectMesh.vertices = Vertices;
	}

	// FixedUpdate se llama 50 veces por segundo, por lo que se llama cada 0,02s
    public void FixedUpdate()
    {
        if (this.Paused)
            return; // Not simulating
        
        // Ejecución de simulación por cada subpaso hasta llegar a los establecidos
        for (int i = 0; i < Substeps; i++)
        {
	        switch (this.IntegrationMethod)
	        {
		        // Select integration method
		        case Integration.Explicit: this.StepExplicit(); break;
		        case Integration.Symplectic: this.StepSymplectic(); break;
		        default:
			        throw new System.Exception("[ERROR] Should never happen!");
	        }
        }
    }

    #endregion

    /// <summary>
    /// Performs a simulation step in 1D using Explicit integration.
    /// </summary>
    private void StepExplicit()
	{
		// Paso 0 - Reset de fuerzas de los nodos
		foreach (Node node in Nodes)
		{
			node.Force = Vector3.zero;
		}
		
		// Paso 1 - Cálculo de fuerzas
		// Nodos
		foreach (Node node in Nodes)
		{
			node.ComputeForces();
		}
		// Muelles
		foreach (Spring spring in Springs)
		{
			spring.ComputeForces();
		}

		// Paso 2 - Integración en el tiempo
		foreach (Node node in Nodes)
		{
			// Si es un nodo fijo continuamos al sigu
			if(node.Fixed)	continue;
			
			// Paso 2.1 - Actualización de posición de cada nodo: x(t+h) = x(t) + h * v(t+h)
			node.Pos = node.Pos + TimeStep * node.Vel;
			
			// Paso 2.2 - Actualización de velocidad de cada nodo: v(t+h) = v(t) + h*(1/m)*F(t)
			node.Vel = node.Vel + TimeStep * (1 / node.Mass) * node.Force;
		}

		// Paso 3 - Actualización longitud del muelle
		foreach (Spring spring in Springs)
		{
			spring.UpdateLength();
		}
	}

	/// <summary>
	/// Performs a simulation step in 1D using Symplectic integration.
	/// </summary>
	private void StepSymplectic()
	{
		// Paso 0 - Reset de fuerzas de los nodos
		foreach (Node node in Nodes)
		{
			node.Force = Vector3.zero;
		}
		
		// Paso 1 - Cálculo de fuerzas
		// Nodos
		foreach (Node node in Nodes)
		{
			node.ComputeForces();
		}
		// Muelles
		foreach (Spring spring in Springs)
		{
			spring.ComputeForces();
		}
		
		// Paso 2 - Integración en el tiempo
		foreach (Node node in Nodes)
		{
			// Si es un nodo fijo continuamos al sigu
			if(node.Fixed)	continue;
			
			// Paso 2.1 - Actualización de velocidad de cada nodo: v(t+h) = v(t) + h*(1/m)*F(t)
			node.Vel = node.Vel + TimeStep * (1 / node.Mass) * node.Force;
			
			// Paso 2.2 - Actualización de posición de cada nodo: x(t+h) = x(t) + h * v(t+h)
			node.Pos = node.Pos + TimeStep * node.Vel;
		}

		// Paso 3 - Actualización longitud del muelle
		foreach (Spring spring in Springs)
		{
			spring.UpdateLength();
		}
	}
}
