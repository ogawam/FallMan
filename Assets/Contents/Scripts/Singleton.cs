using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    static protected T instance = null;
    static public T Get() { 
    	if(instance == null)
    		instance = FindObjectOfType<T>();
    	return instance; 
    }
    void Awake() {
    	instance = this as T;
    }
}