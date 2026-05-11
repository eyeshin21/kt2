using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public int index;

    private Node _node;
    public Node Node
    {
        get
        {
            if (_node == null)
            {
                _node = GetComponent<Node>();
            }

            return _node;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
