using System;
using UnityEngine;

namespace Assets.MapScene.Script.Explorer
{
    public class SelectedElement
    {
        private GameObject container;
        private GameObject Object;
        private Color initialColor;
        private TextMesh TextMesh = null;
        private MeshRenderer MeshRenderer;
        private MeshFilter MeshFilter;

        public SelectedElement(GameObject GameObject)
        {
            this.container = GameObject;
            
            this.MeshRenderer = this.container.GetComponentInChildren<MeshRenderer>();
            this.MeshFilter = this.container.GetComponentInChildren<MeshFilter>();

            this.Object = this.MeshFilter.gameObject;

            this.initialColor = this.MeshRenderer.material.color;
        }

        public Color GetInitialColor()
        {
            return this.initialColor;
        }

        public void SetInitialColor(Color NewColor)
        {
            this.initialColor = NewColor;
        }

        public void SetTextMesh(String Text)
        {
            if (this.TextMesh == null)
            {
                GameObject child = new GameObject(this.Object.name + "-label")
                {
                    tag = "Label"
                };
                child.transform.SetParent(this.container.transform, true);
                child.transform.position = new Vector3(this.container.transform.position.x, this.Object.GetComponent<Collider>().bounds.max.y, this.container.transform.position.z);
                child.transform.localScale = new Vector3(1, 1, 1);

                child.AddComponent<TextMesh>();
                TextMesh textMesh = child.GetComponentInChildren<TextMesh>();
                textMesh.text = Text;
                textMesh.font = Resources.Load("Arial", typeof(Font)) as Font;
                textMesh.fontSize = 10;
                textMesh.fontStyle = FontStyle.Bold;
                textMesh.anchor = TextAnchor.MiddleCenter;
            }
            else
            {
                this.TextMesh.text = Text;
            }
        }

        public Color GetColor()
        {
            return this.MeshRenderer.material.color;
        }

        public Vector3 GetCurrentPosition()
        {
            return this.container.transform.position;
        }

        public void SetCurrentPosition(Vector3 NewPos)
        {
            this.container.transform.position = NewPos;
        }

        public Vector3 GetScale()
        {
            return this.container.transform.localScale;
        }

        public void SetScale(Vector3 NewScale)
        {
            this.container.transform.localScale = NewScale;
        }

        public void AddScale(Vector3 AdditiveScale)
        {
            this.container.transform.localScale += AdditiveScale;
        }

        public Material[] GetMaterials()
        {
            return this.MeshRenderer.materials;
        }

        public MeshRenderer GetMeshRenderer()
        {
            return this.MeshRenderer;
        }

        public MeshFilter GetMeshFilter()
        {
            return this.MeshFilter;
        }

        public GameObject GetContainer()
        {
            return this.container;
        }

        public GameObject GetObject()
        {
            return this.container;
        }

        public String GetDisplayName()
        {
            return this.Object.name.Split('-')[0];
        }

        public String GetObjectName()
        {
            return this.Object.name;
        }

        public String GetContainerName()
        {
            return this.container.name;
        }

        public Collider GetCollider()
        {
            return this.Object.GetComponent<Collider>();
        }

        public void AddPosition(Vector3 AdditivePosition)
        {
            this.container.transform.position += AdditivePosition;
        }

        public void StickToTheFloor()
        {
            Bounds bounds = this.Object.GetComponent<Collider>().bounds;
            Vector3 position = this.container.transform.position;
            float colliderMinY = bounds.min.y;
            if (colliderMinY < 0)
                position += new Vector3(0, -colliderMinY, 0);
            else
                position -= new Vector3(0, colliderMinY, 0);
            this.container.transform.position = position;
        }
    }
}
