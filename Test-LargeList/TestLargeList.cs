using LargeList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Test
{
    public delegate T CreationHandler<T>(Random rand, int MaxIntValue);

    public class TestLargeList<T> where T: IComparable
    {
        public static bool IsStrict = false;

        #region LargeCollection
        public static TestStatus Test_collection()
        {
            TestStatus Status;

            if (!(Status = Test_collection_SimpleInit()).Succeeded)
                return Status;
            else if (!(Status = Test_collection_SimpleInitWithEmptyLargeList()).Succeeded)
                return Status;
            else if (!(Status = Test_collection_SimpleInitWithNullLargeList()).Succeeded)
                return Status;
            else if (!(Status = Test_collection_SimpleInitWithSmallLargeList()).Succeeded)
                return Status;
            else
                return TestStatus.Success;
        }

        private static TestStatus Test_collection_SimpleInit()
        {
            LargeCollection<T> collection;
            string TestName;

            TestName = "collection simple init";
            try
            {
                collection = new LargeCollection<T>();
                if (collection.Count != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = collection as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = collection as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = collection as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == AsILargeCollection)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeCollectionLimits(collection);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_collection_SimpleInitWithEmptyLargeList()
        {
            LargeCollection<T> collection;
            string TestName;

            TestName = "collection simple init with empty list";
            try
            {
                ILargeList<T> initlist = new LargeList<T>();
                collection = new LargeCollection<T>(initlist);
                if (collection.Count != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = collection as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = collection as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = collection as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == AsILargeCollection)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeCollectionLimits(collection);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_collection_SimpleInitWithNullLargeList()
        {
            LargeCollection<T> collection;
            string TestName;

            TestName = "collection simple init with null list";
            try
            {
                ILargeList<T> initlist = null;
                collection = new LargeCollection<T>(initlist);
                return TestStatus.Failed(TestName);
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (IsExceptionEqual(e, "Value cannot be null.\nParameter name: list"))
                        return TestStatus.Success;
                    else
                        return TestStatus.Failed(TestName);
                }
                else
                {
                    if (IsExceptionEqual(e, "Value cannot be null.\nParameter name: collection"))
                        return TestStatus.Success;
                    else
                        return TestStatus.Failed(TestName);
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_collection_SimpleInitWithSmallLargeList()
        {
            LargeCollection<T> collection;
            string TestName;

            TestName = "collection simple init with small list";
            try
            {
                ILargeList<T> initlist = new LargeList<T>();
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                collection = new LargeCollection<T>(initlist);
                if (collection.Count != 7)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = collection as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = collection as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = collection as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == AsILargeCollection)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeCollectionLimits(collection);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus CheckLargeCollectionLimits(LargeCollection<T> collection)
        {
            int Count = (int)collection.Count;

            if (Count > 0)
            {
                try
                {
                    T t = collection[0];
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("get[0] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                T t = collection[-1];
                return TestStatus.Failed("get[-1]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("get[-1] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("get[-1] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    T t = collection[Count - 1];
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("get[Count - 1] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                T t = collection[Count];
                return TestStatus.Failed("get[Count]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("get[Count] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("get[Count] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    collection[0] = default(T);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("set[0] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                collection[-1] = default(T);
                return TestStatus.Failed("set[-1]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("set[-1] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("set[-1] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    collection[Count - 1] = default(T);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("set[Count - 1] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                collection[Count] = default(T);
                return TestStatus.Failed("set[Count]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("set[Count] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("set[Count] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            T[] TestArray = new T[1];

            try
            {
                collection.CopyTo(null, -1);
                return TestStatus.Failed("CopyTo(null, -1)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(null, -1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(null, -1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(null, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.CopyTo(null, 0);
                return TestStatus.Failed("CopyTo(null, 0)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(null, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(null, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(null, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.CopyTo(null, 1);
                return TestStatus.Failed("CopyTo(null, 1)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(null, 1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(null, 1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(null, 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            TestArray = new T[Count + 1];

            try
            {
                collection.CopyTo(TestArray, 0);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(array, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.CopyTo(TestArray, -1);
                return TestStatus.Failed("CopyTo(array, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: dstIndex"))
                        return TestStatus.Failed("CopyTo(array, -1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: arrayIndex"))
                        return TestStatus.Failed("CopyTo(array, -1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(array, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.CopyTo(TestArray, TestArray.Length - Count - 1);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(array, array length-count-1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    collection.CopyTo(TestArray, TestArray.Length - Count);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(array, array length-count) unknown exception: " + e.GetType() + " - " + e.Message);
                }

                try
                {
                    collection.CopyTo(TestArray, TestArray.Length - Count + 1);
                    return TestStatus.Failed("CopyTo(array, array length-count+1)");
                }
                catch (ArgumentException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(array, array length-Count+1) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check arrayIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(array, array length-Count+1) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(array, array length-Count+1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }
            else
            {
                try
                {
                    collection.CopyTo(TestArray, TestArray.Length);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(array, array length) unknown exception: " + e.GetType() + " - " + e.Message);
                }

                try
                {
                    collection.CopyTo(TestArray, TestArray.Length + 1);
                    return TestStatus.Failed("CopyTo(array, array length + 1)");
                }
                catch (ArgumentException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(array, array length + 1) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check arrayIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(array, array length + 1) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(array, array length + 1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                collection.Insert(0, default(T));
                collection.RemoveAt(0);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Insert(0, T) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.Insert(-1, default(T));
                return TestStatus.Failed("Insert(-1, T)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index must be within the bounds of the L" + "ist.\nParameter name: index"))
                    return TestStatus.Failed("Insert(-1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Insert(-1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.Insert(Count, default(T));
                collection.RemoveAt(Count);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Insert(Count, T) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.Insert(Count + 1, default(T));
                return TestStatus.Failed("Insert(Count + 1, T)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index must be within the bounds of the L" + "ist.\nParameter name: index"))
                    return TestStatus.Failed("Insert(Count + 1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Insert(Count + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    collection.RemoveAt(0);
                    collection.Insert(0, default(T));
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("RemoveAt(0) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                collection.RemoveAt(-1);
                return TestStatus.Failed("RemoveAt(-1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("RemoveAt(-1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveAt(-1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    collection.RemoveAt(Count - 1);
                    collection.Insert(Count - 1, default(T));
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("RemoveAt(Count-1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                collection.RemoveAt(Count);
                return TestStatus.Failed("RemoveAt(Count)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("RemoveAt(Count) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveAt(Count) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            return TestStatus.Success;
        }
        #endregion

        #region LargeList
        public static TestStatus Test_list()
        {
            TestStatus Status;

            if (!(Status = Test_list_SimpleInit()).Succeeded)
                return Status;
            else if (!(Status = Test_list_SimpleInitWithZeroCapacity()).Succeeded)
                return Status;
            else if (!(Status = Test_list_SimpleInitWithNegativeCapacity()).Succeeded)
                return Status;
            else if (!(Status = Test_list_SimpleInitWithNonZeroCapacity()).Succeeded)
                return Status;
            else if (!(Status = Test_list_SimpleInitWithNullLargeList()).Succeeded)
                return Status;
            else if (!(Status = Test_list_SimpleInitWithSmallLargeList()).Succeeded)
                return Status;
            else
                return TestStatus.Success;
        }

        private static TestStatus Test_list_SimpleInit()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init";
            try
            {
                list = new LargeList<T>();
                if (list.Count != 0)
                    return TestStatus.Failed(TestName);
                else if (list.Capacity != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = list as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = list as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = list as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == list)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeListLimits(list);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_list_SimpleInitWithZeroCapacity()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init with zero capacity";
            try
            {
                list = new LargeList<T>(0);
                if (list.Count != 0)
                    return TestStatus.Failed(TestName);
                else if (list.Capacity != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = list as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = list as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = list as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == list)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeListLimits(list);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_list_SimpleInitWithNegativeCapacity()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init with negative capacity";
            try
            {
                list = new LargeList<T>(-1);
                return TestStatus.Failed(TestName);
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsExceptionEqual(e, "Non-negative number required.\nParameter name: capacity"))
                    return TestStatus.Success;
                else
                    return TestStatus.Failed(TestName);
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_list_SimpleInitWithNonZeroCapacity()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init with nonzero capacity";
            try
            {
                list = new LargeList<T>(139);
                if (list.Count != 0)
                    return TestStatus.Failed(TestName);
                else if (list.Capacity != 139)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = list as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = list as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = list as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == list)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeListLimits(list);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_list_SimpleInitWithEmptyLargeList()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init with empty list";
            try
            {
                ILargeList<T> initlist = new LargeList<T>();
                list = new LargeList<T>(initlist);
                if (list.Count != 0)
                    return TestStatus.Failed(TestName);
                else if (list.Capacity != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = list as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = list as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = list as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == list)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeListLimits(list);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_list_SimpleInitWithNullLargeList()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init with null list";
            try
            {
                ILargeList<T> initlist = null;
                list = new LargeList<T>(initlist);
                return TestStatus.Failed(TestName);
            }
            catch (ArgumentNullException e)
            {
                if (IsExceptionEqual(e, "Value cannot be null.\nParameter name: collection"))
                    return TestStatus.Success;
                else
                    return TestStatus.Failed(TestName);
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_list_SimpleInitWithSmallLargeList()
        {
            LargeList<T> list;
            string TestName;

            TestName = "list simple init with non empty list";
            try
            {
                ILargeList<T> initlist = new LargeList<T>();
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                list = new LargeList<T>(initlist);
                if (list.Count != 7)
                    return TestStatus.Failed(TestName);
                else if (list.Capacity != 7)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = list as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = list as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = list as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == list)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckLargeListLimits(list);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus CheckLargeListLimits(LargeList<T> list)
        {
            int Count = (int)list.Count;

            if (Count > 0)
            {
                try
                {
                    T t = list[0];
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("get[0] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                T t = list[-1];
                return TestStatus.Failed("get[-1]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("get[-1] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("get[-1] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    T t = list[Count - 1];
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("get[Count-1] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                T t = list[Count];
                return TestStatus.Failed("get[Count]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("get[Count] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("get[Count] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    list[0] = default(T);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("set[0] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list[-1] = default(T);
                return TestStatus.Failed("set[-1]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("set[-1] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("set[-1] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    list[Count - 1] = default(T);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("set[Count-1] unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list[Count] = default(T);
                return TestStatus.Failed("set[Count]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("set[Count] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("set[Count] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Capacity = Count;
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Capacity=Count unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Capacity = Count - 1;
                return TestStatus.Failed("Capacity=Count-1");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "capacity was less than the current size.\nParameter name: value"))
                    return TestStatus.Failed("Capacity=Count-1 exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Capacity=Count-1 unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(0, 0, default(T), null);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(0, 0, T, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(-1, 0, default(T), null);
                return TestStatus.Failed("BinarySearch(-1, 0, T, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: index"))
                    return TestStatus.Failed("BinarySearch(-1, 0, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(-1, 0, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(0, -1, default(T), null);
                return TestStatus.Failed("BinarySearch(0, -1, T, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: count"))
                    return TestStatus.Failed("BinarySearch(0, -1, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(0, -1, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(0, Count, default(T), null);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(0, Count, T, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(0, Count + 1, default(T), null);
                return TestStatus.Failed("BinarySearch(0, Count + 1, T, null)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("BinarySearch(0, Count + 1, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(0, Count + 1, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(Count, 0, default(T), null);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(Count, 0, T, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.BinarySearch(Count + 1, 0, default(T), null);
                return TestStatus.Failed("BinarySearch(Count + 1, 0, T, null)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("BinarySearch(Count + 1, 0, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("BinarySearch(Count + 1, 0, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(null);
                return TestStatus.Failed("CopyTo(null)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(null) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(null) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            T[] TestArray = new T[1];

            if (Count > 1)
            {
                try
                {
                    list.CopyTo(TestArray);
                    return TestStatus.Failed("CopyTo(TestArray)");
                }
                catch (ArgumentException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(TestArray) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check index and count, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(TestArray) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(TestArray) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.CopyTo(null, 0);
                return TestStatus.Failed("CopyTo(null, 0)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(null, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(null, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(null, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                TestArray = new T[Count];

                try
                {
                    list.CopyTo(TestArray, 0);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(TestArray, 0) unknown exception: " + e.GetType() + " - " + e.Message);
                }

                try
                {
                    list.CopyTo(TestArray, 1);
                    return TestStatus.Failed("CopyTo(TestArray, 1)");
                }
                catch (ArgumentException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(TestArray, 1) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Destination array was not long enough. Check arrayIndex and length, and the array's lower bounds."))
                            return TestStatus.Failed("CopyTo(TestArray, 1) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(TestArray, 1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.CopyTo(TestArray, -1);
                return TestStatus.Failed("CopyTo(TestArray, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: dstIndex"))
                        return TestStatus.Failed("CopyTo(TestArray, -1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: arrayIndex"))
                        return TestStatus.Failed("CopyTo(TestArray, -1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(TestArray, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(TestArray, TestArray.Length + 1);
                return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1)");
            }
            catch (ArgumentException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                        return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Destination array was not long enough. Check arrayIndex and length, and the array's lower bounds."))
                        return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(0, null, 0, 0);
                return TestStatus.Failed("CopyTo(0, null, 0, 0)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(0, null, 0, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(0, null, 0, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(0, null, 0, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(-1, TestArray, 0, 0);
                return TestStatus.Failed("CopyTo(-1, TestArray, 0, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: srcIndex"))
                        return TestStatus.Failed("CopyTo(-1, TestArray, 0, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: index"))
                        return TestStatus.Failed("CopyTo(-1, TestArray, 0, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(-1, TestArray, 0, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(0, TestArray, -1, 0);
                return TestStatus.Failed("CopyTo(0, TestArray, -1, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: dstIndex"))
                        return TestStatus.Failed("CopyTo(0, TestArray, -1, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: arrayIndex"))
                        return TestStatus.Failed("CopyTo(0, TestArray, -1, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(0, TestArray, -1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(0, TestArray, 0, -1);
                return TestStatus.Failed("CopyTo(0, TestArray, 0, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: length"))
                        return TestStatus.Failed("CopyTo(0, TestArray, 0, -1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: count"))
                        return TestStatus.Failed("CopyTo(0, TestArray, 0, -1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(0, TestArray, 0, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(Count + 1, TestArray, 0, 0);
                return TestStatus.Failed("CopyTo(Count + 1, TestArray, 0, 0)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("CopyTo(Count + 1, TestArray, 0, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(Count + 1, TestArray, 0, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.CopyTo(0, TestArray, TestArray.Length + 1, 0);
                return TestStatus.Failed("CopyTo(0, TestArray, TestArray.Length + 1, 0)");
            }
            catch (ArgumentException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                        return TestStatus.Failed("CopyTo(0, TestArray, TestArray.Length + 1, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Destination array was not long enough. Check index and count, and the array's lower bounds."))
                        return TestStatus.Failed("CopyTo(0, TestArray, TestArray.Length + 1, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(0, TestArray, TestArray.Length + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 1)
            {
                try
                {
                    list.CopyTo(0, TestArray, 0, TestArray.Length + 1);
                    return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1)");
                }
                catch (ArgumentException e)
                {
                    //if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                    //    return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1) exception: " + e.Message);
                    if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                        return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1) exception: " + e.Message);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }
            else
            {
                try
                {
                    list.CopyTo(0, TestArray, 0, TestArray.Length + 1);
                    return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1)");
                }
                catch (ArgumentException e)
                {
                    if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                        return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1) exception: " + e.Message);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("CopyTo(0, TestArray, 0, TestArray.Length + 1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.CopyTo(0, TestArray, 0, Count + 1);
                return TestStatus.Failed("CopyTo(0, TestArray, 0, Count + 1)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("CopyTo(0, TestArray, 0, Count + 1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(0, TestArray, 0, Count + 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Exists(null);
                return TestStatus.Failed("Exists(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("Exists(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Exists(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Find(null);
                return TestStatus.Failed("Find(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("Find(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Find(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindAll(null);
                return TestStatus.Failed("FindAll(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindAll(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindAll(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(null);
                return TestStatus.Failed("FindIndex(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindIndex(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(0, null);
                return TestStatus.Failed("FindIndex(0, null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindIndex(0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(-1, null);
                return TestStatus.Failed("FindIndex(-1, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindIndex(-1, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(-1, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(-1, (item) => { return true; });
                return TestStatus.Failed("FindIndex(-1,...)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindIndex(-1,...) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(-1,...) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(Count + 1, null);
                return TestStatus.Failed("FindIndex(Count + 1, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindIndex(Count + 1, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(Count + 1, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(Count + 1, (item) => { return true; });
                return TestStatus.Failed("FindIndex(Count + 1, ...)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindIndex(Count + 1, ...) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(Count + 1, ...) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(0, 0, null);
                return TestStatus.Failed("FindIndex(0, 0, null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindIndex(0, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(0, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(-1, 0, null);
                return TestStatus.Failed("FindIndex(-1, 0, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindIndex(-1, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(-1, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(0, -1, null);
                return TestStatus.Failed("FindIndex(0, -1, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Count must be positive and count must refer to a location within the string/array/collection.\nParameter name: count"))
                    return TestStatus.Failed("FindIndex(0, -1, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(0, -1, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindIndex(Count + 1, 0, null);
                return TestStatus.Failed("FindIndex(Count + 1, 0, null)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindIndex(Count + 1, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindIndex(Count + 1, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLast(null);
                return TestStatus.Failed("FindLast(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindLast(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLast(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(null);
                return TestStatus.Failed("FindLastIndex(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindLastIndex(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(0, null);
                return TestStatus.Failed("FindLastIndex(0, null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindLastIndex(0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(-1, null);
                return TestStatus.Failed("FindLastIndex(-1, null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindLastIndex(-1, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(-1, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(-2, (item) => { return true; });
                return TestStatus.Failed("FindLastIndex(-2, ...)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindLastIndex(-2, ...) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(-2, ...) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(Count, (item) => { return true; });
                return TestStatus.Failed("FindLastIndex(Count, ...)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindLastIndex(Count, ...) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(Count, ...) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(0, 0, null);
                return TestStatus.Failed("FindLastIndex(0, 0, null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("FindLastIndex(0, 0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(0, 0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.FindLastIndex(-2, 0, (item) => { return true; });
                return TestStatus.Failed("FindLastIndex(-2, 0, ...)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindLastIndex(-2, 0, ...) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(-2, 0, ...) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 1)
            {
                try
                {
                    list.FindLastIndex(0, -1, (item) => { return true; });
                    return TestStatus.Failed("FindLastIndex(0, -1, ...) with Count>1");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (!IsExceptionEqual(e, "Count must be positive and count must refer to a location within the string/array/collection.\nParameter name: count"))
                        return TestStatus.Failed("FindLastIndex(0, -1, ...) with Count>1 exception: " + e.Message);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("FindLastIndex(0, -1, ...) with Count>1 unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }
            else
            {
                try
                {
                    list.FindLastIndex(0, -1, (item) => { return true; });
                    return TestStatus.Failed("FindLastIndex(0, -1, ...) with Count<=1");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                        return TestStatus.Failed("FindLastIndex(0, -1, ...) with Count<=1 exception: " + e.Message);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("FindLastIndex(0, -1, ...) with Count<=1 unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.FindLastIndex(Count, 0, (item) => { return true; });
                return TestStatus.Failed("FindLastIndex(Count, 0, ...)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("FindLastIndex(Count, 0, ...) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("FindLastIndex(Count, 0, ...) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.ForEach(null);
                return TestStatus.Failed("ForEach(null)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                        return TestStatus.Failed("ForEach(null) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: action"))
                        return TestStatus.Failed("ForEach(null) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("ForEach(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.GetRange(-1, 0);
                return TestStatus.Failed("GetRange(-1, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: index"))
                    return TestStatus.Failed("GetRange(-1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("GetRange(-1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.GetRange(0, -1);
                return TestStatus.Failed("GetRange(0, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: count"))
                    return TestStatus.Failed("GetRange(0, -1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("GetRange(0, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.GetRange(Count + 1, 0);
                return TestStatus.Failed("GetRange(Count + 1, 0)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("GetRange(Count + 1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("GetRange(Count + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.GetRange(0, Count + 1);
                return TestStatus.Failed("GetRange(0, Count + 1)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("GetRange(0, Count + 1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("GetRange(0, Count + 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.IndexOf(default(T), -1);
                return TestStatus.Failed("IndexOf(T, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("IndexOf(0, -1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("IndexOf(0, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.IndexOf(default(T), Count + 1);
                return TestStatus.Failed("IndexOf(T, Count + 1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                        return TestStatus.Failed("IndexOf(0, Count + 1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                        return TestStatus.Failed("IndexOf(0, Count + 1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("IndexOf(0, Count + 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.IndexOf(default(T), -1, 0);
                return TestStatus.Failed("IndexOf(T, -1, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                    return TestStatus.Failed("IndexOf(0, -1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("IndexOf(0, -1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.IndexOf(default(T), Count + 1, 0);
                return TestStatus.Failed("IndexOf(T, Count + 1, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                        return TestStatus.Failed("IndexOf(0, Count + 1, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                        return TestStatus.Failed("IndexOf(0, Count + 1, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("IndexOf(0, Count + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Insert(-1, default(T));
                return TestStatus.Failed("Insert(-1, T)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index must be within the bounds of the L" + "ist.\nParameter name: index"))
                    return TestStatus.Failed("Insert(-1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Insert(-1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Insert(Count + 1, default(T));
                return TestStatus.Failed("Insert(Count + 1, T)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index must be within the bounds of the L" + "ist.\nParameter name: index"))
                    return TestStatus.Failed("Insert(Count + 1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Insert(Count + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.InsertRange(0, null);
                return TestStatus.Failed("InsertRange(0, null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: collection"))
                    return TestStatus.Failed("InsertRange(0, null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("InsertRange(0, null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.InsertRange(-1, TestArray);
                return TestStatus.Failed("InsertRange(-1, TestArray)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("InsertRange(-1, TestArray) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("InsertRange(-1, TestArray) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.InsertRange(Count + 1, TestArray);
                return TestStatus.Failed("InsertRange(Count + 1, TestArray)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("InsertRange(Count + 1, TestArray) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("InsertRange(Count + 1, TestArray) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (!IsStrict)
            {
                try
                {
                    list.LastIndexOf(default(T), -1);
                    return TestStatus.Failed("LastIndexOf(T, -1)");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                            return TestStatus.Failed("LastIndexOf(0, -1) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                            return TestStatus.Failed("LastIndexOf(0, -1) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("LastIndexOf(0, -1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            if (Count > 0)
            {
                try
                {
                    list.LastIndexOf(default(T), Count - 1);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("LastIndexOf(T, Count-1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.LastIndexOf(default(T), Count);
                return TestStatus.Failed("LastIndexOf(T, Count)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                        return TestStatus.Failed("LastIndexOf(0, Count) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                        return TestStatus.Failed("LastIndexOf(0, Count) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("LastIndexOf(0, Count) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    list.LastIndexOf(default(T), Count - 1, 0);
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("LastIndexOf(T, Count - 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            if (!IsStrict)
            {
                try
                {
                    list.LastIndexOf(default(T), -1, 0);
                    return TestStatus.Failed("LastIndexOf(T, -1, 0)");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                            return TestStatus.Failed("LastIndexOf(0, -1, 0) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                            return TestStatus.Failed("LastIndexOf(0, -1, 0) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("LastIndexOf(0, -1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
                }

                try
                {
                    list.LastIndexOf(default(T), Count, 0);
                    return TestStatus.Failed("LastIndexOf(T, Count, 0)");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (IsStrict)
                    {
                        if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                            return TestStatus.Failed("LastIndexOf(0, Count, 0) exception: " + e.Message);
                    }
                    else
                    {
                        if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: startIndex"))
                            return TestStatus.Failed("LastIndexOf(0, Count, 0) exception: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("LastIndexOf(0, Count, 0) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.RemoveAll(null);
                return TestStatus.Failed("RemoveAll(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("RemoveAll(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveAll(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    list.RemoveAt(0);
                    list.Insert(0, default(T));
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("RemoveAt(0) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.RemoveAt(-1);
                return TestStatus.Failed("RemoveAt(-1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("RemoveAt(-1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveAt(-1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            if (Count > 0)
            {
                try
                {
                    list.RemoveAt(Count - 1);
                    list.Insert(Count - 1, default(T));
                }
                catch (Exception e)
                {
                    return TestStatus.Failed("RemoveAt(Count-1) unknown exception: " + e.GetType() + " - " + e.Message);
                }
            }

            try
            {
                list.RemoveAt(Count);
                return TestStatus.Failed("RemoveAt(Count)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("RemoveAt(Count) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveAt(Count) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.RemoveRange(0, 0);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveRange(0, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.RemoveRange(-1, 0);
                return TestStatus.Failed("RemoveRange(-1, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: index"))
                    return TestStatus.Failed("RemoveRange(-1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveRange(-1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.RemoveRange(0, -1);
                return TestStatus.Failed("RemoveRange(0, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: count"))
                    return TestStatus.Failed("RemoveRange(0, -1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveRange(0, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.RemoveRange(Count, 0);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveRange(Count, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.RemoveRange(Count + 1, 0);
                return TestStatus.Failed("RemoveRange(Count + 1, 0)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("RemoveRange(Count + 1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("RemoveRange(Count + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(0, 0);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(0, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(-1, 0);
                return TestStatus.Failed("Reverse(-1, 0)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: index"))
                    return TestStatus.Failed("Reverse(-1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(-1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(0, -1);
                return TestStatus.Failed("Reverse(0, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: count"))
                    return TestStatus.Failed("Reverse(0, -1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(0, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(Count, 0);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(Count, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(Count + 1, 0);
                return TestStatus.Failed("Reverse(Count + 1, 0)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("Reverse(Count + 1, 0) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(Count + 1, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(0, Count);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(0, Count) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Reverse(0, Count + 1);
                return TestStatus.Failed("Reverse(0, Count + 1)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("Reverse(0, Count + 1) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Reverse(0, Count + 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            Comparison<T> comparison = null;

            try
            {
                list.Sort(comparison);
                return TestStatus.Failed("Sort(comparison)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                        return TestStatus.Failed("Sort(comparison) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: comparison"))
                        return TestStatus.Failed("Sort(comparison) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Sort(comparison) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            IComparer<T> comparer = null;

            try
            {
                list.Sort(0, 0, comparer);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Sort(0, 0, comparer) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Sort(-1, 0, comparer);
                return TestStatus.Failed("Sort(-1, 0, comparer)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: index"))
                    return TestStatus.Failed("Sort(-1, 0, comparer) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Sort(-1, 0, comparer) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Sort(0, -1, comparer);
                return TestStatus.Failed("Sort(0, -1, comparer)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Non-negative number required.\nParameter name: count"))
                    return TestStatus.Failed("Sort(0, -1, comparer) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Sort(0, -1, comparer) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Sort(Count, 0, comparer);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Sort(Count, 0, comparer) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.Sort(Count + 1, 0, comparer);
                return TestStatus.Failed("Sort(Count + 1, 0, comparer)");
            }
            catch (ArgumentException e)
            {
                if (!IsExceptionEqual(e, "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."))
                    return TestStatus.Failed("Sort(Count + 1, 0, comparer) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("Sort(Count + 1, 0, comparer) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                list.TrueForAll(null);
                return TestStatus.Failed("TrueForAll(null)");
            }
            catch (ArgumentNullException e)
            {
                if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: match"))
                    return TestStatus.Failed("TrueForAll(null) exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("TrueForAll(null) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            return TestStatus.Success;
        }
        #endregion

        #region Read-only LargeCollection
        public static TestStatus Test_readonly_collection()
        {
            TestStatus Status;

            if (!(Status = Test_readonly_collection_WithEmptyLargeList()).Succeeded)
                return Status;
            else if (!(Status = Test_readonly_collection_WithNullLargeList()).Succeeded)
                return Status;
            else if (!(Status = Test_readonly_collection_WithSmallLargeList()).Succeeded)
                return Status;
            else
                return TestStatus.Success;
        }

        private static TestStatus Test_readonly_collection_WithEmptyLargeList()
        {
            ReadOnlyLargeCollection<T> rcollection;
            string TestName;

            TestName = "readonly collection simple init with empty list";
            try
            {
                ILargeList<T> initlist = new LargeList<T>();
                rcollection = new ReadOnlyLargeCollection<T>(initlist);
                if (rcollection.Count != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = rcollection as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (!AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = rcollection as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (!AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (!AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = rcollection as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == AsILargeCollection)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckReadOnlyLargeCollectionLimits(rcollection);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_readonly_collection_WithNullLargeList()
        {
            ReadOnlyLargeCollection<T> rcollection;
            string TestName;

            TestName = "readonly collection simple init with null list";
            try
            {
                ILargeList<T> initlist = null;
                rcollection = new ReadOnlyLargeCollection<T>(initlist);
                return TestStatus.Failed(TestName);
            }
            catch (ArgumentNullException e)
            {
                if (IsExceptionEqual(e, "Value cannot be null.\nParameter name: list"))
                    return TestStatus.Success;
                else
                    return TestStatus.Failed(TestName);
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_readonly_collection_WithSmallLargeList()
        {
            ReadOnlyLargeCollection<T> rcollection;
            string TestName;

            TestName = "readonly collection simple init with small list";
            try
            {
                ILargeList<T> initlist = new LargeList<T>();
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                rcollection = new ReadOnlyLargeCollection<T>(initlist);
                if (rcollection.Count != 7)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsILargeCollectionG = rcollection as ILargeCollection<T>;
                    if (AsILargeCollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (!AsILargeCollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsILargeList = rcollection as ILargeList;
                            if (AsILargeList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (!AsILargeList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (!AsILargeList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsILargeCollection = rcollection as ILargeCollection;
                                    if (AsILargeCollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsILargeCollection.SyncRoot == AsILargeCollection)
                                        return TestStatus.Failed(TestName);
                                    else
                                    {
                                        TestStatus Status = CheckReadOnlyLargeCollectionLimits(rcollection);
                                        if (!Status.Succeeded)
                                            return Status;
                                        else
                                            return TestStatus.Success;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus CheckReadOnlyLargeCollectionLimits(ReadOnlyLargeCollection<T> collection)
        {
            int Count = (int)collection.Count;

            try
            {
                T t = collection[-1];
                return TestStatus.Failed("get[-1]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("get[-1] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("get[-1] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                T t = collection[Count];
                return TestStatus.Failed("get[Count]");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (!IsExceptionEqual(e, "Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index"))
                    return TestStatus.Failed("get[Count] exception: " + e.Message);
            }
            catch (Exception e)
            {
                return TestStatus.Failed("get[Count] unknown exception: " + e.GetType() + " - " + e.Message);
            }

            T[] TestArray = new T[1];

            try
            {
                collection.CopyTo(null, 0);
                return TestStatus.Failed("CopyTo(null, 0)");
            }
            catch (ArgumentNullException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: dest"))
                        return TestStatus.Failed("CopyTo(null, 0) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Value cannot be null.\nParameter name: array"))
                        return TestStatus.Failed("CopyTo(null, 0) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(null, 0) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.CopyTo(TestArray, -1);
                return TestStatus.Failed("CopyTo(TestArray, -1)");
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: dstIndex"))
                        return TestStatus.Failed("CopyTo(TestArray, -1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Number was less than the array's lower bound in the first dimension.\nParameter name: index"))
                        return TestStatus.Failed("CopyTo(TestArray, -1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(TestArray, -1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            try
            {
                collection.CopyTo(TestArray, TestArray.Length + 1);
                return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1)");
            }
            catch (ArgumentException e)
            {
                if (IsStrict)
                {
                    if (!IsExceptionEqual(e, "Destination array was not long enough. Check destIndex and length, and the array's lower bounds."))
                        return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1) exception: " + e.Message);
                }
                else
                {
                    if (!IsExceptionEqual(e, "Destination array was not long enough. Check index and length, and the array's lower bounds."))
                        return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1) exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                return TestStatus.Failed("CopyTo(TestArray, TestArray.Length + 1) unknown exception: " + e.GetType() + " - " + e.Message);
            }

            return TestStatus.Success;
        }
        #endregion

        #region Read-only List
        public static TestStatus Test_readonly_list()
        {
            TestStatus Status;

            if (!(Status = Test_readonly_list_WithEmptyList()).Succeeded)
                return Status;
            else if (!(Status = Test_readonly_list_WithNullList()).Succeeded)
                return Status;
            else if (!(Status = Test_readonly_list_WithSmallList()).Succeeded)
                return Status;
            else
                return TestStatus.Success;
        }

        private static TestStatus Test_readonly_list_WithEmptyList()
        {
            ReadOnlyLargeList<T> rlist;
            string TestName;

            TestName = "readonly list simple init with empty list";
            try
            {
                LargeList<T> initlist = new LargeList<T>();
                rlist = new ReadOnlyLargeList<T>(initlist);
                if (rlist.Count != 0)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsICollectionG = rlist as ILargeCollection<T>;
                    if (AsICollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (!AsICollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsIList = rlist as ILargeList;
                            if (AsIList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (!AsIList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (!AsIList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsICollection = rlist as ILargeCollection;
                                    if (AsICollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsICollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsICollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsICollection.SyncRoot == AsICollection)
                                        return TestStatus.Failed(TestName);
                                    else if (!CheckReadOnlyListLimits(rlist))
                                        return TestStatus.Failed(TestName);
                                    else
                                        return TestStatus.Success;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_readonly_list_WithNullList()
        {
            ReadOnlyLargeList<T> rlist;
            string TestName;

            TestName = "readonly list simple init with null list";
            try
            {
                LargeList<T> initlist = null;
                rlist = new ReadOnlyLargeList<T>(initlist);
                return TestStatus.Failed(TestName);
            }
            catch (ArgumentNullException e)
            {
                if (IsExceptionEqual(e, "Value cannot be null.\nParameter name: list"))
                    return TestStatus.Success;
                else
                    return TestStatus.Failed(TestName);
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }

        private static TestStatus Test_readonly_list_WithSmallList()
        {
            ReadOnlyLargeList<T> rlist;
            string TestName;

            TestName = "readonly list simple init with small list";
            try
            {
                LargeList<T> initlist = new LargeList<T>();
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                initlist.Add(default(T));
                rlist = new ReadOnlyLargeList<T>(initlist);
                if (rlist.Count != 7)
                    return TestStatus.Failed(TestName);
                else
                {
                    ILargeCollection<T> AsICollectionG = rlist as ILargeCollection<T>;
                    if (AsICollectionG == null)
                        return TestStatus.Failed(TestName);
                    else
                    {
                        if (!AsICollectionG.IsReadOnly)
                            return TestStatus.Failed(TestName);
                        else
                        {
                            ILargeList AsIList = rlist as ILargeList;
                            if (AsIList == null)
                                return TestStatus.Failed(TestName);
                            else
                            {
                                if (!AsIList.IsFixedSize)
                                    return TestStatus.Failed(TestName);
                                else if (!AsIList.IsReadOnly)
                                    return TestStatus.Failed(TestName);
                                else
                                {
                                    ILargeCollection AsICollection = rlist as ILargeCollection;
                                    if (AsICollection == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsICollection.IsSynchronized)
                                        return TestStatus.Failed(TestName);
                                    else if (AsICollection.SyncRoot == null)
                                        return TestStatus.Failed(TestName);
                                    else if (AsICollection.SyncRoot == AsICollection)
                                        return TestStatus.Failed(TestName);
                                    else if (!CheckReadOnlyListLimits(rlist))
                                        return TestStatus.Failed(TestName);
                                    else
                                        return TestStatus.Success;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return TestStatus.Failed(TestName);
            }
        }
        #endregion

        #region Test
        private static void PrintDiagnostic(string Line)
        {
#if DEBUG
            Debug.Print(Line);
#else
            Debug.WriteLine(Line);
            //Console.Write(Line);
            //Console.WriteLine();
#endif
        }

        private static void PassTest(string TestName)
        {
            PrintDiagnostic("Pass: " + TestName);
        }

        private static void FailTest(string TestName)
        {
            PrintDiagnostic("Fail: " + TestName);
        }

        private static bool IsExceptionEqual(Exception e, string Message)
        {
            return e.Message.Replace("\r\n", "\n") == Message;
        }

        public static void Init(bool AssemblyIsStrict, int DefaultMaxSegmentCapacity)
        {
            IsStrict = AssemblyIsStrict;
            //IsStrict = true;
        }

        public static TestStatus TestAll(CreationHandler<T> handler)
        {
            TestStatus Status = Test(handler);
            if (!Status.Succeeded)
                FailTest(Status.Name);

            return Status;
        }

        private static TestStatus Test(CreationHandler<T> handler)
        {
            TestStatus Status;

            if (!(Status = Test_collection()).Succeeded)
                return Status;
            else if (!(Status = Test_list()).Succeeded)
                return Status;
            else if (!(Status = Test_readonly_collection()).Succeeded)
                return Status;
            else if (!(Status = Test_readonly_list()).Succeeded)
                return Status;
            else if (!(Status = SimultaneousTest_collections(100, handler)).Succeeded)
                return Status;
            else if (!(Status = SimultaneousTest_lists(30, handler)).Succeeded)
                return Status;
            else
                return TestStatus.Success;
        }

        private static void InitSeed(int Loop, out Random rand)
        {
            rand = new Random(Loop);
        }

        private static int CompareTwoObjects(T o1, T o2)
        {
            if (o1 == null && o2 == null)
                return 0;
            else if (o1 == null && o2 != null)
                return 1;
            else if (o1 != null && o2 == null)
                return -1;
            else
                return o1.CompareTo(o2);
        }

        private const int MaxIntValue = 100;
        private const int MaxSize = 50;
        private const int ClearOperationOdds = 10;
        private const int ReverseOperationOdds = 3;
        private const int SortOperationOdds = 3;
        #endregion

        #region Collection Comparison
        private enum CollectionOperation
        {
            Set,
            Add,
            Clear,
            Insert,
            Remove,
            RemoveAt,
        }

        private static TestStatus SimultaneousTest_collections(int MaxLoops, CreationHandler<T> handler)
        {
            PrintDiagnostic("Comparing Collection<T> and LargeCollection<T>");

            TestStatus Status;
            for (int Loop = 0; Loop < MaxLoops; Loop++)
                if (!(Status = SimultaneousTest_collections(Loop, MaxLoops, handler)).Succeeded)
                    return Status;

            return TestStatus.Success;
        }

        public static TestStatus SimultaneousTest_collections(int Loop, int MaxLoops, CreationHandler<T> handler)
        {
            TestStatus Status;
            int MaxSteps = 50;

            if (MaxLoops > 0)
                PrintDiagnostic("Loop #" + (Loop + 1) + "/" + MaxLoops);
            else
                PrintDiagnostic("Loop #" + (Loop + 1));

            Collection<T> small_collection = new Collection<T>();
            LargeCollection<T> large_collection = new LargeCollection<T>();

            Random rand;
            InitSeed(Loop, out rand);

            for (int Step = 0; Step < MaxSteps; Step++)
            {
                if (!(Status = UpdateTest_collection(small_collection, large_collection, Loop, Step, rand, handler)).Succeeded)
                    return Status;

                if (!(Status = IsEqual_collections(small_collection, large_collection, Loop, Step)).Succeeded)
                    return Status;
            }

            return TestStatus.Success;
        }

        private static TestStatus UpdateTest_collection(Collection<T> small_collection, LargeCollection<T> large_collection, int Loop, int Step, Random rand, CreationHandler<T> handler)
        {
            int Count = small_collection.Count;
            int OperationMax = typeof(CollectionOperation).GetEnumValues().Length;
            CollectionOperation Operation = (CollectionOperation)rand.Next(OperationMax);
            T Item;
            int Odds, Index;

            switch (Operation)
            {
                case CollectionOperation.Set:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count);
                        Item = handler(rand, MaxIntValue);
                        small_collection[Index] = Item;
                        large_collection[Index] = Item;
                    }
                    return TestStatus.Success;

                case CollectionOperation.Add:
                    if (Count < MaxSize)
                    {
                        Item = handler(rand, MaxIntValue);
                        small_collection.Add(Item);
                        large_collection.Add(Item);
                    }
                    return TestStatus.Success;

                case CollectionOperation.Clear:
                    Odds = rand.Next(ClearOperationOdds);
                    if (Odds == 0)
                    {
                        small_collection.Clear();
                        large_collection.Clear();
                    }
                    return TestStatus.Success;

                case CollectionOperation.Insert:
                    if (Count < MaxSize)
                    {
                        Index = rand.Next(Count + 1);
                        Item = handler(rand, MaxIntValue);
                        small_collection.Insert(Index, Item);
                        large_collection.Insert(Index, Item);
                    }
                    return TestStatus.Success;

                case CollectionOperation.Remove:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count * 2);
                        if (Index < Count)
                            Item = small_collection[Index];
                        else
                            Item = handler(rand, MaxIntValue);
                        bool SmallRemoved = small_collection.Remove(Item);
                        bool LargeRemoved = large_collection.Remove(Item);
                        if (SmallRemoved != LargeRemoved)
                            return TestStatus.Failed("Collection<" + typeof(T).Name + ">, Remove, " + "Loop#" + Loop + ", Step#" + Step);
                    }
                    return TestStatus.Success;

                case CollectionOperation.RemoveAt:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count);
                        small_collection.RemoveAt(Index);
                        large_collection.RemoveAt(Index);
                    }
                    return TestStatus.Success;

                default:
                    throw new InvalidOperationException();
            }
        }

        private static TestStatus IsEqual_collections(Collection<T> small_collection, LargeCollection<T> large_collection, int Loop, int Step)
        {
            if (small_collection.Count != large_collection.Count)
                return TestStatus.Failed("Collection<" + typeof(T).Name + ">, compare Count, " + "Loop#" + Loop + ", Step#" + Step);

            int Count = small_collection.Count;

            for (int i = 0; i < Count; i++)
                if (!Equals(small_collection[i], large_collection[i]))
                    return TestStatus.Failed("Collection<" + typeof(T).Name + ">, compare getter, " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                if (small_collection.Contains(small_collection[i]) != large_collection.Contains(large_collection[i]))
                    return TestStatus.Failed("Collection<" + typeof(T).Name + ">, compare Contains, " + "Loop#" + Loop + ", Step#" + Step);

            T t = default(T);
            if (small_collection.Contains(t) != large_collection.Contains(t))
                return TestStatus.Failed("Collection<" + typeof(T).Name + ">, compare Contains, " + "Loop#" + Loop + ", Step#" + Step);

            T[] small_array = new T[Count];
            T[] large_array = new T[Count];
            small_collection.CopyTo(small_array, 0);
            large_collection.CopyTo(large_array, 0);

            for (int i = 0; i < Count; i++)
                if (!Equals(small_array[i],large_array[i]))
                    return TestStatus.Failed("Collection<" + typeof(T).Name + ">, compare CopyTo, " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                if (small_collection.IndexOf(small_collection[i]) != large_collection.IndexOf(large_collection[i]))
                    return TestStatus.Failed("Collection<" + typeof(T).Name + ">, compare IndexOf, " + "Loop#" + Loop + ", Step#" + Step);

            return TestStatus.Success;
        }
        #endregion

        #region List Comparison
        private enum ListOperation
        {
            Set,
            SetCapacity,
            Add,
            AddRange,
            Clear,
            Insert,
            InsertRange,
            Remove,
            RemoveAll,
            RemoveAt,
            RemoveRange,
            Reverse,
            Sort,
            TrimExcess,
        }

        private static TestStatus SimultaneousTest_lists(int MaxLoops, CreationHandler<T> handler)
        {
            PrintDiagnostic("Comparing List<T> and LargeList<T>");

            TestStatus Status;

            for (int Loop = 0; Loop < MaxLoops; Loop++)
                if (!(Status = SimultaneousTest_lists(Loop, MaxLoops, handler)).Succeeded)
                    return Status;

            return TestStatus.Success;
        }

        public static TestStatus SimultaneousTest_lists(int Loop, int MaxLoops, CreationHandler<T> handler)
        {
            TestStatus Status;
            int MaxSteps = 50;

            if (MaxLoops > 0)
                PrintDiagnostic("Loop #" + (Loop + 1) + "/" + MaxLoops);
            else
                PrintDiagnostic("Loop #" + (Loop + 1));

            List<T> small_list = new List<T>();
            LargeList<T> large_list = new LargeList<T>();

            Random rand;
            InitSeed(Loop, out rand);

            for (int Step = 0; Step < MaxSteps; Step++)
            {
                if (!(Status = UpdateTest_list(small_list, large_list, Loop, Step, rand, handler)).Succeeded)
                    return Status;

                if (!(Status = IsEqual_lists(small_list, large_list, Loop, Step)).Succeeded)
                    return Status;
            }

            return TestStatus.Success;
        }

        private static TestStatus UpdateTest_list(List<T> small_list, LargeList<T> large_list, int Loop, int Step, Random rand, CreationHandler<T> handler)
        {
            int Count = small_list.Count;
            int OperationMax = typeof(ListOperation).GetEnumValues().Length;
            ListOperation Operation = (ListOperation)rand.Next(OperationMax);
            T Item;
            int Odds, Index, Size;

            //PrintDiagnostic("Executing: " + Operation);

            switch (Operation)
            {
                case ListOperation.Set:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count);
                        Item = handler(rand, MaxIntValue);
                        small_list[Index] = Item;
                        large_list[Index] = Item;
                    }
                    return TestStatus.Success;

                case ListOperation.SetCapacity:
                    if (Count < MaxSize)
                    {
                        Size = Count + rand.Next(MaxSize - Count);
                        small_list.Capacity = Size;
                        large_list.Capacity = Size;
                    }
                    return TestStatus.Success;

                case ListOperation.Add:
                    if (Count < MaxSize)
                    {
                        Item = handler(rand, MaxIntValue);
                        small_list.Add(Item);
                        large_list.Add(Item);
                    }
                    return TestStatus.Success;

                case ListOperation.AddRange:
                    if (Count < MaxSize)
                    {
                        Size = rand.Next(MaxSize - Count);
                        if (Size > 0)
                        {
                            List<T> collection = new List<T>();
                            for (int i = 0; i < Size; i++)
                            {
                                Item = handler(rand, MaxIntValue);
                                collection.Add(Item);
                            }
                            small_list.AddRange(collection);
                            large_list.AddRange(collection);
                        }
                    }
                    return TestStatus.Success;

                case ListOperation.Clear:
                    Odds = rand.Next(ClearOperationOdds);
                    if (Odds == 0)
                    {
                        small_list.Clear();
                        large_list.Clear();
                    }
                    return TestStatus.Success;

                case ListOperation.Insert:
                    if (Count < MaxSize)
                    {
                        Index = rand.Next(Count + 1);
                        Item = handler(rand, MaxIntValue);
                        small_list.Insert(Index, Item);
                        large_list.Insert(Index, Item);
                    }
                    return TestStatus.Success;

                case ListOperation.InsertRange:
                    if (Count < MaxSize)
                    {
                        Size = rand.Next(MaxSize - Count);
                        if (Size > 0)
                        {
                            Index = rand.Next(Count + 1);
                            List<T> collection = new List<T>();
                            for (int i = 0; i < Size; i++)
                            {
                                Item = handler(rand, MaxIntValue);
                                collection.Add(Item);
                            }
                            small_list.InsertRange(Index, collection);
                            large_list.InsertRange(Index, collection);
                        }
                    }
                    return TestStatus.Success;

                case ListOperation.Remove:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count * 2);
                        if (Index < Count)
                            Item = small_list[Index];
                        else
                            Item = handler(rand, MaxIntValue);
                        bool SmallRemoved = small_list.Remove(Item);
                        bool LargeRemoved = large_list.Remove(Item);
                        if (SmallRemoved != LargeRemoved)
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, Remove, " + "Loop#" + Loop + ", Step#" + Step);
                    }
                    return TestStatus.Success;

                case ListOperation.RemoveAll:
                    if (Count > 0)
                    {
                        T MaxItem = handler(rand, MaxSize);
                        long SmallRemoved = small_list.RemoveAll((item) => { return CompareTwoObjects(item, MaxItem) > 0; });
                        long LargeRemoved = large_list.RemoveAll((item) => { return CompareTwoObjects(item, MaxItem) > 0; });
                        if (SmallRemoved != LargeRemoved)
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, RemoveAll, " + "Loop#" + Loop + ", Step#" + Step);
                    }
                    return TestStatus.Success;

                case ListOperation.RemoveAt:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count);
                        small_list.RemoveAt(Index);
                        large_list.RemoveAt(Index);
                    }
                    return TestStatus.Success;

                case ListOperation.RemoveRange:
                    if (Count > 0)
                    {
                        Index = rand.Next(Count);
                        Size = rand.Next(Count - Index);
                        if (Size > 0)
                        {
                            small_list.RemoveRange(Index, Size);
                            large_list.RemoveRange(Index, Size);
                        }
                    }
                    return TestStatus.Success;

                case ListOperation.Reverse:
                    Odds = rand.Next(ReverseOperationOdds);
                    if (Odds == 0)
                    {
                        small_list.Reverse();
                        small_list.Reverse();
                    }
                    else
                    {
                        Index = rand.Next(Count);
                        Size = rand.Next(Count - Index);
                        if (Size > 0)
                        {
                            small_list.Reverse(Index, Size);
                            large_list.Reverse(Index, Size);
                        }
                    }
                    return TestStatus.Success;

                case ListOperation.Sort:
                    Odds = rand.Next(SortOperationOdds);
                    if (Odds == 0)
                    {
                        small_list.Sort();
                        large_list.Sort();
                    }
                    else
                    {
                        Comparer<T> comparer;
                        if (Odds > SortOperationOdds / 2)
                            comparer = Comparer<T>.Create((T item1, T item2) => { return CompareTwoObjects(item2, item1); });
                        else
                            comparer = Comparer<T>.Create((T item1, T item2) => { return CompareTwoObjects(item1, item2); });
                        Index = rand.Next(Count);
                        Size = rand.Next(Count - Index);
                        if (Size > 0)
                        {
                            small_list.Sort(Index, Size, comparer);
                            large_list.Sort(Index, Size, comparer);
                        }
                    }
                    return TestStatus.Success;

                case ListOperation.TrimExcess:
                    small_list.TrimExcess();
                    large_list.TrimExcess();
                    return TestStatus.Success;

                default:
                    throw new InvalidOperationException();
            }
        }

        private static TestStatus IsEqual_lists(List<T> small_list, LargeList<T> large_list, int Loop, int Step)
        {
            if (small_list.Capacity >= 4 && small_list.Capacity * 3 < large_list.Capacity)
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare Capacity, " + "Loop#" + Loop + ", Step#" + Step);

            if (small_list.Count != large_list.Count)
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare Count, " + "Loop#" + Loop + ", Step#" + Step);

            int Count = small_list.Count;

            for (int i = 0; i < Count; i++)
                if (!Equals(small_list[i], large_list[i]))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare getter, " + "Loop#" + Loop + ", Step#" + Step);

            List<T> small_sorted_list = new List<T>(small_list);
            LargeList<T> large_sorted_list = new LargeList<T>(large_list);
            small_sorted_list.Sort();
            large_sorted_list.Sort();

            T t = default(T);
            long SmallSearchresult = small_sorted_list.BinarySearch(t);
            long LargeSearchresult = large_sorted_list.BinarySearch(t);
            if (SmallSearchresult != LargeSearchresult)
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare BinarySearch(item) (" + SmallSearchresult + ", " + LargeSearchresult + "), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
            {
                SmallSearchresult = small_sorted_list.BinarySearch(small_sorted_list[i]);
                LargeSearchresult = large_sorted_list.BinarySearch(large_sorted_list[i]);
                if (SmallSearchresult != LargeSearchresult)
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare BinarySearch(item#" + i +") (" + SmallSearchresult + ", " + LargeSearchresult + "), " + "Loop#" + Loop + ", Step#" + Step);
            }

            Comparer<T> comparer;
            comparer = Comparer<T>.Create((T item1, T item2) => { return CompareTwoObjects(item2, item1); });
            small_sorted_list.Sort(comparer);
            large_sorted_list.Sort(comparer);

            SmallSearchresult = small_list.BinarySearch(t, comparer);
            LargeSearchresult = large_list.BinarySearch(t, comparer);
            if (SmallSearchresult != LargeSearchresult)
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare BinarySearch(item, comparer) (" + SmallSearchresult + ", " + LargeSearchresult + "), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
            {
                SmallSearchresult = small_list.BinarySearch(small_list[i], comparer);
                LargeSearchresult = large_list.BinarySearch(large_list[i], comparer);
                if (SmallSearchresult != LargeSearchresult)
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare BinarySearch(item#" + i + ", comparer) (" + SmallSearchresult + ", " + LargeSearchresult + "), " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                    if (small_list.BinarySearch(j, k, t, comparer) != large_list.BinarySearch(j, k, t, comparer))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare BinarySearch(index, count, item, comparer), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                for (int j = 0; j < Count; j++)
                    for (int k = 0; j + k < Count; k++)
                        if (small_list.BinarySearch(j, k, small_list[i], comparer) != large_list.BinarySearch(j, k, large_list[i], comparer))
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, compare BinarySearch(index, count, item, comparer), " + "Loop#" + Loop + ", Step#" + Step);

            if (small_list.Contains(t) != large_list.Contains(t))
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare Contains, " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                if (small_list.Contains(small_list[i]) != large_list.Contains(large_list[i]))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare Contains, " + "Loop#" + Loop + ", Step#" + Step);

            Converter<T, int> converter = (T item) => { return MaxSize - 1 - item.GetHashCode(); };
            List<int> small_converted = small_list.ConvertAll(converter);
            LargeList<int> large_converted = large_list.ConvertAll(converter);
            for (int i = 0; i < Count; i++)
                if (small_converted[i] != large_converted[i])
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare ConvertAll, " + "Loop#" + Loop + ", Step#" + Step);

            T[] small_array;
            T[] large_array;

            small_array = new T[Count];
            large_array = new T[Count];
            small_list.CopyTo(small_array);
            large_list.CopyTo(large_array);

            for (int i = 0; i < Count; i++)
                if (!Equals(small_array[i], large_array[i]))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare CopyTo(array), " + "Loop#" + Loop + ", Step#" + Step);

            for (int j = 0; j < Count; j++)
            {
                small_array = new T[Count + j];
                large_array = new T[Count + j];
                small_list.CopyTo(small_array, j);
                large_list.CopyTo(large_array, j);

                for (int i = 0; i < Count; i++)
                    if (!Equals(small_array[j + i], large_array[j + i]))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare CopyTo(array,index), " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                    for (int l = 0; l < Count; l++)
                    {
                        small_array = new T[Count + l];
                        large_array = new T[Count + l];
                        small_list.CopyTo(j, small_array, l, k);
                        large_list.CopyTo(j, large_array, l, k);

                        for (int i = 0; i < k; i++)
                            if (!Equals(small_array[l + i], large_array[l + i]))
                                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare CopyTo(index,array,index,count), " + "Loop#" + Loop + ", Step#" + Step);
                    }

            Predicate<T> match;

            for (int i = 0; i < MaxIntValue; i++)
            {
                match = (item) => { return (item.GetHashCode() & 0xF) == i; };
                if (small_list.Exists(match) != large_list.Exists(match))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare Exists, " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int i = 0; i < MaxIntValue; i++)
            {
                match = (item) => { return item != null && (item.GetHashCode() % MaxIntValue) == i; };
                if (!Equals(small_list.Find(match), large_list.Find(match)))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare Find, " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int j = 0; j < MaxIntValue; j++)
            {
                match = (item) => { return (item.GetHashCode() % MaxIntValue) == j; };
                List<T> small_find = small_list.FindAll(match);
                LargeList<T> large_find = large_list.FindAll(match);

                if (small_find.Count != large_find.Count)
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindAll, " + "Loop#" + Loop + ", Step#" + Step);
                else
                {
                    int FindCount = small_find.Count;
                    for (int i = 0; i < FindCount; i++)
                        if (!Equals(small_find[i], large_find[i]))
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindAll, " + "Loop#" + Loop + ", Step#" + Step);
                }
            }

            for (int i = 0; i < MaxIntValue; i++)
            {
                match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                if (small_list.FindIndex(match) != large_list.FindIndex(match))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindIndex, " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int j = 0; j < Count; j++)
                for (int i = 0; i < MaxIntValue; i++)
                {
                    match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                    if (small_list.FindIndex(j, match) != large_list.FindIndex(j, match))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindIndex(index,match), " + "Loop#" + Loop + ", Step#" + Step);
                }

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                    for (int i = 0; i < MaxIntValue; i++)
                    {
                        match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                        if (small_list.FindIndex(j, k, match) != large_list.FindIndex(j, k, match))
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindIndex(index,count,match), " + "Loop#" + Loop + ", Step#" + Step);
                    }

            for (int i = 0; i < MaxIntValue; i++)
            {
                match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                if (!Equals(small_list.FindLast(match), large_list.FindLast(match)))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindLast, " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int i = 0; i < MaxIntValue; i++)
            {
                match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                if (small_list.FindLastIndex(match) != large_list.FindLastIndex(match))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindLastIndex, " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int j = 0; j < Count; j++)
                for (int i = 0; i < MaxIntValue; i++)
                {
                    match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                    long SmallFindResult = small_list.FindLastIndex(Count - 1 - j, match);
                    long LargeFindResult = large_list.FindLastIndex(Count - 1 - j, match);

                    if (SmallFindResult != LargeFindResult)
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindLastIndex(index,match), " + "Loop#" + Loop + ", Step#" + Step);
                }

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                    for (int i = 0; i < MaxIntValue; i++)
                    {
                        match = (item) => { return (item.GetHashCode() % MaxIntValue) == i; };
                        if (small_list.FindLastIndex(Count - 1 - j, k, match) != large_list.FindLastIndex(Count - 1 - j, k, match))
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, compare FindLastIndex(index,count,match), " + "Loop#" + Loop + ", Step#" + Step);
                    }

            int Total;
            Action<T> action = (item) => { Total = (item.GetHashCode() % MaxIntValue) * 1103515245 + 12345; };
            Total = 1;
            small_list.ForEach(action);
            int SmallTotal = Total;
            Total = 1;
            large_list.ForEach(action);
            int LargeTotal = Total;

            if (SmallTotal != LargeTotal)
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare ForEach, " + "Loop#" + Loop + ", Step#" + Step);

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                {
                    List<T> small_range = small_list.GetRange(j, k);
                    LargeList<T> large_range = large_list.GetRange(j, k);

                    if (small_range.Count != large_range.Count)
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare GetRange, " + "Loop#" + Loop + ", Step#" + Step);
                    else
                    {
                        int FindCount = small_range.Count;
                        for (int i = 0; i < FindCount; i++)
                            if (!Equals(small_range[i], large_range[i]))
                                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare GetRange, " + "Loop#" + Loop + ", Step#" + Step);
                    }
                }

            if (small_list.IndexOf(t) != large_list.IndexOf(t))
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare IndexOf, " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                if (small_list.IndexOf(small_list[i]) != large_list.IndexOf(large_list[i]))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare IndexOf, " + "Loop#" + Loop + ", Step#" + Step);

            for (int j = 0; j < Count; j++)
                if (small_list.IndexOf(t, j) != large_list.IndexOf(t, j))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare IndexOf(item,index), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                for (int j = 0; j < Count; j++)
                    if (small_list.IndexOf(small_list[i], j) != large_list.IndexOf(large_list[i], j))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare IndexOf(item,index), " + "Loop#" + Loop + ", Step#" + Step);

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                    if (small_list.IndexOf(t, j, k) != large_list.IndexOf(t, j, k))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare IndexOf(item,index,count), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                for (int j = 0; j < Count; j++)
                    for (int k = 0; j + k < Count; k++)
                        if (small_list.IndexOf(small_list[i], j, k) != large_list.IndexOf(large_list[i], j, k))
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, compare IndexOf(item,index,count), " + "Loop#" + Loop + ", Step#" + Step);

            if (small_list.LastIndexOf(t) != large_list.LastIndexOf(t))
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare LastIndexOf, " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                if (small_list.LastIndexOf(small_list[i]) != large_list.LastIndexOf(large_list[i]))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare LastIndexOf, " + "Loop#" + Loop + ", Step#" + Step);

            for (int j = 0; j < Count; j++)
                if (small_list.LastIndexOf(t, Count - 1 - j) != large_list.LastIndexOf(t, Count - 1 - j))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare LastIndexOf(item,index), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                for (int j = 0; j < Count; j++)
                    if (small_list.LastIndexOf(small_list[i], Count - 1 - j) != large_list.LastIndexOf(large_list[i], Count - 1 - j))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare LastIndexOf(item,index), " + "Loop#" + Loop + ", Step#" + Step);

            for (int j = 0; j < Count; j++)
                for (int k = 0; j + k < Count; k++)
                    if (small_list.LastIndexOf(t, Count - 1 - j, k) != large_list.LastIndexOf(t, Count - 1 - j, k))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare LastIndexOf(item,index,count), " + "Loop#" + Loop + ", Step#" + Step);

            for (int i = 0; i < Count; i++)
                for (int j = 0; j < Count; j++)
                    for (int k = 0; j + k < Count; k++)
                        if (small_list.LastIndexOf(small_list[i], Count - 1 - j, k) != large_list.LastIndexOf(large_list[i], Count - 1 - j, k))
                            return TestStatus.Failed("List<" + typeof(T).Name + ">, compare LastIndexOf(item,index,count), " + "Loop#" + Loop + ", Step#" + Step);

            small_array = small_list.ToArray();
            large_array = large_list.ToArray();
            if (small_array.Length != large_array.Length)
                return TestStatus.Failed("List<" + typeof(T).Name + ">, compare ToArray, " + "Loop#" + Loop + ", Step#" + Step);
            else
            {
                int ArrayLength = small_array.Length;
                for (int i = 0; i < ArrayLength; i++)
                    if (!Equals(small_array[i], large_array[i]))
                        return TestStatus.Failed("List<" + typeof(T).Name + ">, compare ToArray, " + "Loop#" + Loop + ", Step#" + Step);
            }

            for (int i = 0; i < MaxIntValue; i++)
            {
                match = (item) => { return (item.GetHashCode() % MaxIntValue) != i; };
                if (small_list.TrueForAll(match) != large_list.TrueForAll(match))
                    return TestStatus.Failed("List<" + typeof(T).Name + ">, compare TrueForAll, " + "Loop#" + Loop + ", Step#" + Step);
            }

            return TestStatus.Success;
        }
        #endregion

        private static bool CheckReadOnlyListLimits(ReadOnlyLargeList<T> list)
        {
            return true;
        }
    }
}
