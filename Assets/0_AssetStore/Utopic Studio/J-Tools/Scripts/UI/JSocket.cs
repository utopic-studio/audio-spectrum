using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace J
{
    /// <summary>
    /// Manages a UI socket for connecting lines to other sockets. Can allow different connection policies.
    /// </summary>
    [AddComponentMenu("J/UI/JSocket")]
    [RequireComponent(typeof(LineRenderer))]
    public class JSocket : JBase
    {

        public enum SocketConnectionPolicy
        {
            Free,
            DisallowSameType,
            DisallowDifferentType
        }

        //Line renderer used to join socket ports
        private LineRenderer lineRender;

        //True when the cursor is setup on mid-operation
        private bool bUsingCursor;

        //"Port" that connects to a socket
        private JSocket OutPort;

        //Current socket being managed
        private static JSocket CurrentSocket;

        //Type used for distinguishing which kind of port this is
        public string TypeIdentifier;

        //Current offset of the cursor (to be improved)
        public float cursorOffset = 2.0f;

        //Connection policy
        public SocketConnectionPolicy ConnectionPolicy;

        // Use this for initialization
        void Start()
        {
            //Obtain the renderer
            lineRender = GetComponent<LineRenderer>();

            if (!lineRender)
                Debug.LogError("No line renderer found!");
        }

        // Update is called once per frame
        void Update()
        {
            ConfigureRenderer();
        }

        private void ConfigureRenderer()
        {
            if (bUsingCursor)
            {
                Vector3[] positions = new Vector3[2];
                positions[0] = transform.position;
                positions[1] = GetCameraCursorPosition();
                lineRender.SetPositions(positions);
                lineRender.positionCount = 2;
            }
            else if (OutPort)
            {
                Vector3[] positions = new Vector3[2];
                positions[0] = transform.position;
                positions[1] = OutPort.transform.position;
                lineRender.SetPositions(positions);
                lineRender.positionCount = 2;
            }
            else
            {
                lineRender.positionCount = 0;
            }
        }

        public void HandleClick()
        {
            if (JSocket.CurrentSocket && JSocket.CurrentSocket.Equals(this))
            {
                CloseOperation();
            }
            else if (JSocket.CurrentSocket != null)
            {
                //Should try to connect
                TryConnectSocket(JSocket.CurrentSocket);
                CloseOperation();
            }
            else
            {
                //Should begin connect operation
                BeginOperation(this);
            }
        }

        private Vector3 GetCameraCursorPosition()
        {
            Vector3 cameraForward = Camera.main.transform.forward;


            return Camera.main.transform.position + cameraForward * cursorOffset;
        }


        //Uses main camera direction
        private static void BeginOperation(JSocket NewCurrent)
        {
            //Clear old port binding
            NewCurrent.ClearOutPort();

            //Close any active operation
            CloseOperation();

            CurrentSocket = NewCurrent;
            NewCurrent.bUsingCursor = true;
        }

        private static void CloseOperation()
        {
            if (CurrentSocket)
                CurrentSocket.bUsingCursor = false;

            CurrentSocket = null;
        }

        public void ClearOutPort()
        {
            if (OutPort && OutPort.OutPort)
            {
                //Avoid calling the method on the other socket, so we avoid infinite recursion
                OutPort.OutPort = null;
                OutPort = null;
            }
        }

        public bool TryConnectSocket(JSocket target)
        {
            //Test for connection policy
            if (AllowsConnectionOneDirection(target) && target.AllowsConnectionOneDirection(this))
            {
                ClearOutPort();
                OutPort = target;
                target.OutPort = this;

                return true;
            }

            return false;
        }

        public bool AllowsConnectionOneDirection(JSocket target)
        {
            switch (ConnectionPolicy)
            {
                case SocketConnectionPolicy.Free:
                    return true;
                case SocketConnectionPolicy.DisallowDifferentType:
                    return TypeIdentifier.Equals(target.TypeIdentifier);
                case SocketConnectionPolicy.DisallowSameType:
                    return !TypeIdentifier.Equals(target.TypeIdentifier);
                default:
                    return false;

            }
        }
    }
}