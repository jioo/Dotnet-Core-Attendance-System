using System.Collections;
using System.Collections.Generic;
using WebApi.Features.Config;

namespace Test.Api.ClassData
{
    public class InvalidConfigModels : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            
            // error: Time In is required
            yield return new object[] 
            { 
                new ConfigViewModel 
                { 
                    TimeIn = "",
                    TimeOut = "17:00",
                    GracePeriod = 10,
                } 
            };
            
            // error: Time Out is required
            yield return new object[] 
            { 
                new ConfigViewModel 
                { 
                    TimeIn = "25:00",
                    TimeOut = "",
                    GracePeriod = 10,
                } 
            };

            // error: Invalid TimeIn
            yield return new object[] 
            { 
                new ConfigViewModel 
                { 
                    TimeIn = "25:00",
                    TimeOut = "17:00",
                    GracePeriod = 10,
                } 
            };

            // error: Invalid TimeOut
            yield return new object[] 
            { 
                new ConfigViewModel 
                { 
                    TimeIn = "09:00",
                    TimeOut = "25:00",
                    GracePeriod = 10,
                }
            };

            // error: Invalid GracePeriod
            yield return new object[] 
            { 
                new ConfigViewModel 
                { 
                    TimeIn = "09:00",
                    TimeOut = "17:00",
                    GracePeriod = 60,
                }
            };

            // error: Invalid GracePeriod
            yield return new object[] 
            { 
                new ConfigViewModel 
                { 
                    TimeIn = "09:00",
                    TimeOut = "17:00",
                    GracePeriod = -1,
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}