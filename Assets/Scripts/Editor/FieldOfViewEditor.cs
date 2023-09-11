using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FastCampus.AI
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        void OnSceneGUI()
        {
            // target: 인스팩터 창 위의 obj => FieldOfView 타입캐스팅을 통한 스크립트 얻어오기
            FieldOfView fov = (FieldOfView)target; 

            // Draw view radius
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

            // 양 시야각의 끝점
            Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

            // NearestTarget이 아닌 모든 target을 붉은 색으로 표기
            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.VisibleTargets)
            {
                if (fov.NearestTarget != visibleTarget)
                {
                    Handles.DrawLine(fov.transform.position, visibleTarget.position);
                }
            }

            // NearestTarget만 녹색으로 표기
            Handles.color = Color.green;
            if (fov.NearestTarget)
            {
                Handles.DrawLine(fov.transform.position, fov.NearestTarget.position);
            }
        }
    }
}