using System.Collections;
using System.Collections.Generic;
using WebApi.Features.Accounts;

namespace Test.Api.ClassData
{
    public class InvalidRegisterModels : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // error: UserName field is required
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "", 
                    Password = "valid_password", 
                    FullName = "Valid Name", 
                    CardNo = "valid_cardNo", 
                } 
            };
            
            // error: Password field is required
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "valid_username", 
                    Password = "", 
                    FullName = "Valid Name", 
                    CardNo = "valid_cardNo", 
                } 
            };
            
            // error: The Password must be at least 6 characters long.
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "valid_username", 
                    Password = "12345", 
                    FullName = "Valid Name", 
                    CardNo = "valid_cardNo", 
                } 
            };
            
            // error: FullName field is required
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "valid_username", 
                    Password = "valid_password", 
                    FullName = "", 
                    CardNo = "valid_cardNo", 
                } 
            };
            
            // error: CardNo field is required
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "valid_username", 
                    Password = "valid_password", 
                    FullName = "Valid Name", 
                    CardNo = "", 
                } 
            };

            // error: UserName is already taken.
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "existing_username", 
                    Password = "valid_password", 
                    FullName = "Valid Name", 
                    CardNo = "valid_cardNo", 
                } 
            };

            // error: Card No. is already in use
            yield return new object[] 
            { 
                new RegisterViewModel 
                { 
                    UserName = "valid_username", 
                    Password = "valid_password", 
                    FullName = "Valid Name", 
                    CardNo = "existing_cardNo", 
                } 
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}