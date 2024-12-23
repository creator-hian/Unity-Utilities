using System;
using System.Reflection;

namespace UnityUtilities
{
    public static class EventUtils
    {
        /// <summary>
        /// 객체에 있는 이벤트의 모든 호출을 지웁니다.
        /// </summary>
        /// <typeparam name="T">객체의 타입.</typeparam>
        /// <param name="obj">객체.</param>
        /// <param name="eventName">이벤트 이름.</param>
        public static void ClearEventInvocations<T>(this T obj, string eventName)
            where T : class
        {
            if (obj == null)
            {
                return;
            }

            var fi = GetEventField(obj.GetType(), eventName);
            if (fi == null)
                return;
            fi.SetValue(obj, null);
        }

        /// <summary>
        /// 이벤트 필드를 가져옵니다.
        /// </summary>
        /// <param name="type">객체의 타입.</param>
        /// <param name="eventName">이벤트 이름.</param>
        /// <returns>이벤트 필드 정보.</returns>
        private static FieldInfo GetEventField(Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                // 필드로 정의된 이벤트 찾기
                field = type.GetField(
                    eventName,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic
                );
                if (
                    field != null
                    && (
                        field.FieldType == typeof(MulticastDelegate)
                        || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))
                    )
                )
                {
                    break;
                }

                // 속성으로 정의된 이벤트 찾기 { add; remove; }
                var eventInfo = type.GetEvent(
                    eventName,
                    BindingFlags.Static
                        | BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic
                );
                if (eventInfo != null)
                {
                    field = eventInfo.DeclaringType.GetField(
                        eventInfo.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic
                    );
                    if (field == null)
                    {
                        field = eventInfo.DeclaringType.BaseType.GetField(
                            eventInfo.Name,
                            BindingFlags.Instance | BindingFlags.NonPublic
                        );
                    }
                    if (field != null)
                    {
                        break;
                    }
                }
                type = type.BaseType;
            }

            return field;
        }
    }
}
