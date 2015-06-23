
namespace DynamicJSONserializer
{


	class MainClass
	{


		public class DynamicArguments
		{
            System.Collections.Generic.Dictionary<string, object> dict;

			public System.Collections.IEnumerable Keys 
			{
				get{ 
					return dict.Keys;
				}

			}


			public DynamicArguments()
			{
                dict = new System.Collections.Generic.Dictionary<string, object>();
			}

			public DynamicArguments(string json)
			{
                dict = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(json);
			}


			public T GetValue<T>(string key)
			{
				bool bIsNullable = false;
				bool hasValue = dict.ContainsKey(key);
				System.Type t = typeof(T);

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Nullable<>))
				{
                    t = System.Nullable.GetUnderlyingType(t);
					bIsNullable = true;
				}

				if (hasValue)
				{
                    string value = System.Convert.ToString( dict[key], System.Globalization.CultureInfo.InvariantCulture);

					if (t == typeof(string))
						return (T)(object) value;
					
					if (!string.IsNullOrEmpty(value))
					{
						if (t == typeof(System.Int32))
                            return (T)(object)  System.Int32.Parse(value);

						if (t == typeof(System.UInt32))
                            return (T)(object)  System.UInt32.Parse(value);
						
						if (t == typeof(System.Int64))
                            return (T)(object)  System.Int64.Parse(value);

						if (t == typeof(System.UInt64))
                            return (T)(object)  System.UInt64.Parse(value);
						
						if (t == typeof(double))
							return (T)(object)  double.Parse(value);

						if (t == typeof(float))
							return (T)(object)  float.Parse(value);

                        if (t == typeof(System.Guid))
							return (T)(object)new System.Guid(value);

						if (t == typeof(bool))
						{
							bool bReturnValue = false;

							if (bool.TryParse(value, out bReturnValue))
								return (T)(object)bReturnValue;

							if (value == "0")
								return (T)(object)false;

                            if(System.StringComparer.OrdinalIgnoreCase.Equals(value,"YES"))
								return (T)(object)true;

                            if(System.StringComparer.OrdinalIgnoreCase.Equals(value,"NO"))
								return (T)(object)false;
							
                            System.Int64 lng;
                            if (System.Int64.TryParse(value, out lng))
                                return (T) (object) true;

							double dbl;
							if (double.TryParse(value, out dbl))
                                return (T)(object) true;
							
							return (T)(object)false;
						}

                        if (t == typeof(System.DateTime))
						{
							if((value.IndexOf('T') != -1) || (value.IndexOf('/') != -1 ))
                                return (T)(object)  System.DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture);

							if(value.IndexOf('.') != -1)
                                return (T) (object) System.DateTime.ParseExact(value, "dd.MM.yyyy", new System.Globalization.CultureInfo("de-CH", false));

                            return (T)(object)  System.DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
						} // End if (t == typeof(DateTime))

                        if(t == typeof(System.Enum))
                            return (T) (object) System.Enum.Parse(t, value);
						
                        if (t == typeof(DynamicArguments))
                            return (T)(object) (new DynamicArguments(value));
                        
					} // End if (!string.IsNullOrEmpty(value))

				} // End if (hasValue)

				if (bIsNullable)
					return (T) (object) null;

				T val = default(T);
				return 	(T) (object) val;
			} // End Function GetValue<T>(key)


		} // End Class


		public static void Main (string[] args)
		{
            Newtonsoft.Json.JsonSerializerSettings jss = new Newtonsoft.Json.JsonSerializerSettings();
            jss.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(new System.DateTime(2013, 12, 31), jss);
			System.Console.WriteLine(str);



            string json = @"{   ""testobj"": {""foo"": ""bar"", ""lol"": ""rofl""},   ""stichtag_msft"": ""\/Date(1388444400000+0100)\/"", ""stichtag_deCH"": ""15.03.2015"", ""stichtag_jsondate"": ""2015-06-22T18:02:00.725Z"",  ""gb_uid"":""2ba62b36-8b30-457c-8946-82fa452c99fb"",""key2"":""value2"", ""key3"" : 123, ""TSK_DatumVon"" : ""2013-01-01"", ""key5"": true, ""so_uid"": null }";




			DynamicArguments da = new DynamicArguments(json);

			foreach(var k in da.Keys)
			{
                System.Console.WriteLine(k);
			}

            string testobj = da.GetValue<string>("testobj");
            System.Console.WriteLine(testobj);


            DynamicArguments da2 = da.GetValue<DynamicArguments>("testobj");
            string foo = da2.GetValue<string>("foo");
            System.Console.WriteLine(foo);

            System.DateTime dt = da.GetValue<System.DateTime>("stichtag");
            System.Console.WriteLine(dt);


			// Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            System.Collections.Generic.Dictionary<string, dynamic> values = 
                Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, dynamic>>(json);

            foreach (System.Collections.Generic.KeyValuePair<string, dynamic> kvp in values)
			{
                System.Console.WriteLine(kvp.Key);
                System.Console.WriteLine(kvp.Value.GetType());
			}

            System.Console.WriteLine(" --- Press any key to continue --- ");

		}
	}
}
