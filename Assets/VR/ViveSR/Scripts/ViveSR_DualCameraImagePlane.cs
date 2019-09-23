//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;

namespace Vive.Plugin.SR
{
    [RequireComponent(typeof(MeshFilter))]
    public class ViveSR_DualCameraImagePlane : MonoBehaviour
    {
        public DualCameraIndex CameraIndex;

        [Header("Internal parameters")]
        public int DistortedImageWidth = 0;
        public int DistortedImageHeight = 0;
        public int UndistortedImageWidth = 0;
        public int UndistortedImageHeight = 0;
        public double DistortedCx = 0f;
        public double DistortedCy = 0f;
        public double UndistortedCx = 0f;
        public double UndistortedCy = 0f;
        public double FocalLength = 0f;
        public float[] UndistortionMap;
        public int MeshResolutionShrinkingRatio = 4;

        private int NumVertexRows = 0;
        private int NumVertexCols = 0;

        [Header("Undistortion option")]
        [Tooltip("Please read the code comment for how to use this option.")]
        // Please set this option before entering play mode.
        // To change to the old method, please follow the steps:
        // 1.  Before entering play mode, uncheck this option for all image planes.
        // 2.  Enter play mode.
        // 3.  At the game object "DualCamera (Head)"'s "Vive SR_Dual Camera Image Renderer" component,
        //     click "Don't Update Camera Material" and then click "Update Undistorted Material".
        public bool UseNewUndistortionMethod = true;

        public void Initial()
        {
            if (UseNewUndistortionMethod)
            {
                if (MeshResolutionShrinkingRatio < 1)
                {
                    MeshResolutionShrinkingRatio = 1;
                }
                NumVertexRows = DistortedImageHeight / MeshResolutionShrinkingRatio;
                NumVertexCols = DistortedImageWidth / MeshResolutionShrinkingRatio;
                CreateMesh(NumVertexRows, NumVertexCols);
                UndistortMesh();
            }
            else
            {
                SetUV(CameraIndex);
                SetCorrectSize();
            }
        }

        /// <summary>
        /// Create a mesh to display the camera image.
        /// </summary>
        private void CreateMesh(int numVertexRows, int numVertexCols)
        {
            // Mesh topology, where (i,j) is the row-major vertex index:
            //  +--> x
            //  |
            //  v    (0,0)--(0,1)--(0,2)-- ..
            //  -y     |   /  |   /  |   /
            //         |  /   |  /   |  /
            //       (1,0)--(1,1)--(1,2)-- ..
            //         |   /  |   /  |   /
            //         ..     ..     ..

            // Create vertices, set UVs, and create normals.
            Vector3[] vertices = new Vector3[numVertexRows * numVertexCols];
            Vector2[] uvs = new Vector2[numVertexRows * numVertexCols];
            Vector3[] normals = new Vector3[numVertexRows * numVertexCols];
            // The plane sits on the xy plane and points towards -z.
            float xMin = -1.0f;
            float yMin = -1.0f;
            float xWidth = 2.0f;
            float yWidth = 2.0f;
            for (int i = 0; i < numVertexRows; ++i)
            {
                for (int j = 0; j < numVertexCols; ++j)
                {
                    float u = (float) j / (numVertexCols - 1);
                    float v = (float) i / (numVertexRows - 1);
                    Vector3 vertex = new Vector3(xMin + u * xWidth, yMin + yWidth - v * yWidth, 0.0f);
                    Vector2 uv = new Vector2(u, v);
                    Vector3 normal = new Vector3(0.0f, 0.0f, -1.0f);
                    int vertexIndex = numVertexCols * i + j;
                    vertices[vertexIndex] = vertex;
                    uvs[vertexIndex] = uv;
                    normals[vertexIndex] = normal;
                }
            }

            // Create triangles.
            int[] triangles = new int[(numVertexRows - 1) * (numVertexCols - 1) * 2 * 3];
            for (int i = 0; i < numVertexRows - 1; ++i)
            {
                for (int j = 0; j < numVertexCols - 1; ++j)
                {
                    int triangleIndex = ((numVertexCols - 1) * i + j) * 2 * 3;
                    int vertex00Index = numVertexCols * i + j;
                    int vertex01Index = vertex00Index + 1;
                    int vertex10Index = vertex00Index + numVertexCols;
                    int vertex11Index = vertex10Index + 1;
                    triangles[triangleIndex + 0] = vertex00Index;
                    triangles[triangleIndex + 1] = vertex01Index;
                    triangles[triangleIndex + 2] = vertex10Index;
                    triangles[triangleIndex + 3] = vertex10Index;
                    triangles[triangleIndex + 4] = vertex01Index;
                    triangles[triangleIndex + 5] = vertex11Index;
                }
            }

            // Set the mesh by these mesh components.
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.triangles = triangles;
        }

        /// <summary>
        /// Deform the image plane mesh according to the undistortion map in order to undistort the camera image.
        /// </summary>
        private void UndistortMesh()
        {
            // Transform from pixel space to image plane space.
            // Pixel space:
            //     origin: the upper-left corner of the image
            //     x axis: the local x axis
            //     y axis: the local y axis
            //     z axis: the local z axis
            //     unit: 1 pixel
            // Image plane space:
            //     origin: the principal point of the image scaled from pixel-unit to meter-unit
            //     x axis: the local x axis
            //     y axis: the local y axis
            //     z axis: the local z axis
            //     unit: 1 meter

            // Create vectors with zero z values.
            // Convert from right-handed coordinates to the left-handed coordinates.
            // Also create uvs.
            Vector3[] pixelVertices = new Vector3[NumVertexRows * NumVertexCols];
            Vector2[] uvs = new Vector2[NumVertexRows * NumVertexCols];
            for (int i = 0; i < NumVertexRows; ++i)
            {
                for (int j = 0; j < NumVertexCols; ++j)
                {
                    int undistortionMapIndex = (i * MeshResolutionShrinkingRatio * DistortedImageWidth + j * MeshResolutionShrinkingRatio) * 4;
                    float distortedX = UndistortionMap[undistortionMapIndex];
                    float distortedY = UndistortionMap[undistortionMapIndex + 1];
                    float undistortedX = UndistortionMap[undistortionMapIndex + 2];
                    float undistortedY = UndistortionMap[undistortionMapIndex + 3];
                    pixelVertices[i * NumVertexCols + j] = new Vector3(undistortedX, -1 * undistortedY, 0f);
                    uvs[i * NumVertexCols + j] = new Vector2(distortedX / (float) DistortedImageWidth, distortedY / (float) DistortedImageHeight);
                }
            }

            // Calculate the length of a pixel in the real space unit.
            float imagePlaneDistance = transform.localPosition.z;
            float pixelLength = imagePlaneDistance / (float) FocalLength;

            // Represent the principal point in the left-handed coordinates in the scaled pixel space.
            Vector3 principalPointInScaledPixelSpace = new Vector3((float) UndistortedCx * pixelLength, -1 * (float) UndistortedCy * pixelLength, 0f);

            // Calculate the transformation from the pixel space to the image plane space.
            Vector3 scaleFromPixelSpaceToImagePlaneSpace = new Vector3(pixelLength, pixelLength, pixelLength);
            Matrix4x4 transformationFromPixelSpaceToImagePlaneSpace = Matrix4x4.TRS(-1 * principalPointInScaledPixelSpace, Quaternion.identity, scaleFromPixelSpaceToImagePlaneSpace);

            // Transform the vectors and set to the mesh vertices.
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] meshVertices = mesh.vertices;
            for (int i = 0; i < pixelVertices.Length; ++i)
            {
                meshVertices[i] = transformationFromPixelSpaceToImagePlaneSpace.MultiplyPoint3x4(pixelVertices[i]);
            }
            mesh.vertices = meshVertices;
            mesh.uv = uvs;
        }

        /// <summary>
        /// Set the scale of the image plane according to the image size and the focal length.
        /// </summary>
        private void SetCorrectSize()
        {
            float imageWidth = UndistortedImageWidth;
            float imageHeight = UndistortedImageHeight;
            float imageAspectRatio = imageWidth / imageHeight;

            // Get the distance of the image plane to the camera.
            // ASSUME the plane is in the z direction of the camera.
            float imagePlaneDisanceZ = transform.localPosition.z;
            float focalLength = (float)FocalLength;

            // Calculate the correct size of the image plane according to the size of
            // the original images, the image plane distance and the focal length.
            Mesh planeMesh = GetComponent<MeshFilter>().mesh;
            Vector3[] originalVertices = planeMesh.vertices;  // Get a copy of the vertices of the plane.
            Vector3 upperRightMostVertex = new Vector3(-1e8f, -1e8f, 0);
            Vector3 lowerLeftMostVertex = new Vector3(1e8f, 1e8f, 0);
            for (int i = 0; i < originalVertices.Length; i++)
            {
                upperRightMostVertex = Vector3.Max(upperRightMostVertex, originalVertices[i]);
                lowerLeftMostVertex = Vector3.Min(lowerLeftMostVertex, originalVertices[i]);
            }
            float imagePlaneWidth = upperRightMostVertex.x - lowerLeftMostVertex.x;
            float imagePlaneHeight = upperRightMostVertex.y - lowerLeftMostVertex.y;
            float imagePlaneAspectRatio = imagePlaneWidth / imagePlaneHeight;
            // Create the transformation matrices.
            // Translate to the geometric center.
            Vector3 geometricCenter = (upperRightMostVertex + lowerLeftMostVertex) / 2;
            Matrix4x4 translationToGeomatricCenter = Matrix4x4.TRS(-1 * geometricCenter, Quaternion.identity, Vector3.one);
            // Scale x and y to fit the vertical FOV.
            float fovScaleFactor = ((imageHeight / focalLength) * imagePlaneDisanceZ) / imagePlaneHeight;
            Matrix4x4 scalingForCorrectFov = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(fovScaleFactor, fovScaleFactor, 1));
            // Scale x to fit the aspect ratio.
            float aspectRatioScaleFactor = imageAspectRatio / imagePlaneAspectRatio;
            Matrix4x4 scalingForCorrectAspectRatio = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(aspectRatioScaleFactor, 1, 1));
            // Translate back to the origin by the inverse matrix.
            // Combine all transformations.
            Matrix4x4 transformationForOriginalImage = translationToGeomatricCenter.inverse * scalingForCorrectAspectRatio * scalingForCorrectFov * translationToGeomatricCenter;
            // Apply the transformation.
            for (int i = 0; i < originalVertices.Length; i++)
            {
                originalVertices[i] = transformationForOriginalImage.MultiplyPoint3x4(originalVertices[i]);
            }
            // Assign the vertices for the correct image plane size.
            planeMesh.vertices = originalVertices;
        }

        /// <summary>
        /// Mirror the uv because the origin point of images of Unity is defferent from the source image.
        /// </summary>
        /// <param name="eye"></param>
        private void SetUV(DualCameraIndex eye)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Vector2[] srcUV = meshFilter.mesh.uv;
            Vector2[] dstUV = new Vector2[srcUV.Length];
            for (int i = 0; i < srcUV.Length; i++)
                dstUV[i] = new Vector2(srcUV[i].x, srcUV[srcUV.Length - i - 1].y);
            meshFilter.mesh.uv = dstUV;
        }
    }
}