using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosureManager : MonoBehaviour {

    [SerializeField]
    private List<GameObject> m_closures = new List<GameObject>();
    //private List<GameObject> closed = new List<GameObject>();

    public void ToClose(List<int> idsToClose) {
        foreach (GameObject c in m_closures) {
            Closure closure = c.GetComponent<Closure>();

            if (idsToClose.Contains(closure.id)) {
                closure.ToClose();
            }
        }
    }

    public void Closed(List<int> idsClosed) {
        foreach (GameObject c in m_closures) {
            Closure closure = c.GetComponent<Closure>();
            if(idsClosed.Contains(closure.id)) {
                closure.Closed();
            }
        }
    }

    public void Open(List<int> idsToOpen) {
        foreach (GameObject c in m_closures) {
            Closure closure = c.GetComponent<Closure>();

            if(idsToOpen.Contains(closure.id)) {
                closure.Open();
            }
        }
    }

}
