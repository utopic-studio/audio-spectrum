using UnityEngine;

namespace J
{
		
	public static class JExtensionMethods
    {
        
        #region GAME_OBJECT

        /// <summary>
        /// Activa/desactiva un objeto en la jerarquia
        /// </summary>
        public static void JToggleActive(this GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }

        /// <summary>
        /// Crea un objeto. Si ya existía, no se crea y se usa ese objeto
        /// </summary>
        /// <param name="name">Nombre del objeto en la jerarquía</param>
        /// <returns>Objeto creado o el que ya existía</returns>
        public static GameObject JCreateEmptyGameObjectNoDuplicates(this GameObject go, string name)
        {
            GameObject newGameObject;
            newGameObject = GameObject.Find(name);
            if (!newGameObject)
            {
                newGameObject = new GameObject(name);
            }
            return newGameObject;
        }

        #endregion



        #region TRANSFORM

        /// <summary>
        /// Reinicia la posición, rotación y la escala
        /// </summary>
        public static void JReset(this Transform t) {
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
		}

        /// <summary>
        /// Asigna un valor para transform.x
        /// </summary>
		public static void JSetX(this Transform t, float x) {
			t.position = new Vector3(x, t.position.y, t.position.z);
		}

        /// <summary>
        /// Asigna un valor para transform.y
        /// </summary>
        public static void JSetY(this Transform t, float y) {
			t.position = new Vector3(t.position.x, y, t.position.z);
		}

        /// <summary>
        /// Asigna un valor para transform.z
        /// </summary>
        public static void JSetZ(this Transform t, float z) {
			t.position = new Vector3(t.position.x, t.position.y, z);
		}

        /// <summary>
        /// Obtiene el padre de este objeto - Muestra warning en consola si no tiene padre
        /// </summary>
        /// <returns>Padre o null</returns>
        public static Transform JGetParent(this Transform t) {
			if (!t.parent)
				Debug.LogWarning(string.Format("J - No parent found for object {0}", t.gameObject.name));
			return t.parent;
		}

        #endregion
        


        #region COLLIDER

        
        /// <summary>
        /// Activa/desactiva el componente Collider
        /// </summary>
        public static void JToggleCollider(this Collider col) {
			col.enabled = !col.enabled;
		}

        /// <summary>
        /// Activa/desactiva si es el Collider es trigger o no
        /// </summary>
        public static void JToggleIsTrigger(this Collider col) {
			col.isTrigger = !col.isTrigger;
		}

        #endregion
        


        #region RENDERER

        /// <summary>
        /// Activa/desactiva el componente Renderer
        /// </summary>
        public static void JToggleRenderer(this Renderer rend) {
			rend.enabled = !rend.enabled;
		}

        #endregion



        #region RIGIDBODY
        
        /// <summary>
        /// Activa/desactiva si este Rigidbody es afectado por la gravedad
        /// </summary>
        public static void JToggleUseGravity(this Rigidbody rb) {
			rb.useGravity = !rb.useGravity;
		}
        
        /// <summary>
        /// Activa/desactiva si este Rigidbody es kinemático o no
        /// </summary>
        public static void JToggleIsKinematic(this Rigidbody rb) {
			rb.isKinematic = !rb.isKinematic;
		}

        #endregion

    }

}