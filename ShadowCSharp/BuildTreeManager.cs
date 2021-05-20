using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets._01_Basic_ShadowMap.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.ShadowCSharp
{
    class Data
    {
        public Octree root;
        public int depth;
        public bool flag;
        public int sz;
        public int hashval;
        public int childId;
        public int step;

        public Data(Octree Root, int Depth, bool Flag, int Size, int Hashval, int ChildId, int Step = 0)
        {
            root = Root;
            depth = Depth;
            flag = Flag;
            sz = Size;
            hashval = Hashval;
            childId = ChildId;
            step = Step;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("depth = {0}\n", depth);
            str.AppendFormat("step = {0}\n", step);
            return str.ToString();
        }
    }

    class MyStack
    {
        private List<Data> _list;
        private int length = 0;

        public MyStack()
        {
            length = 0;
            _list = new List<Data>();
        }

        public void Push(Data data)
        {
            if (_list.Count > length)
                _list[length] = data;
            else
                _list.Add(data);
            ++length;
        }

        public void Push(Octree Root, int Depth, bool Flag, int Size, int Hashval, int ChildId, int Step = 0)
        {
            if (_list.Count > length)
            {
                _list[length].root = Root;
                _list[length].depth = Depth;
                _list[length].flag = Flag;
                _list[length].sz = Size;
                _list[length].hashval = Hashval;
                _list[length].childId = ChildId;
                _list[length].step = Step;
            }
            else
            {
                _list.Add(new Data(Root,Depth,Flag,Size,Hashval,ChildId,Step));
            }

            ++length;
        }

        public Data Pop()
        {
            if (length == 0)
            {
                Debug.LogError("No Data");
                return null;
            }

            --length;
            return _list[length];
        }

        public void Clear()
        {
            _list.Clear();
            length = 0;
        }

        public int Count
        {
            get { return length; }
        }
    }

    partial class OctreeManager
    {
        private MyStack dataStack = new MyStack();
        //private Stack<Data> dataStack = new Stack<Data>();
        private int hashCnt;

        private int mergeCnt = 0;
        private int physicsMergeCnt = 0;
        private int leavesCnt = 0;

        public void BuildTree(int depth,out Octree allroot)
        {
            stk.Clear();
            hashCnt = 0;
            Data last = null;
            allroot = new Octree();
            Data tree = new Data(allroot, depth, true, 1, 0, 0);
            dataStack.Push(tree);
            while (dataStack.Count > 0)
            {

                Data top = dataStack.Pop();
                switch (top.step)
                {
                    case 0:
                        //判断叶子节点返回
                        if (top.depth == 0)
                        {
                            Vector3 pos = GetWorldPositionByStack();
                            //Debug.Log(pos);
                            top.root.SubTree.Clear();
                            top.root.InShadow = (CommonValues.GetShadowState(pos) < 0.1f);
                            top.root = top.root.InShadow ? _inShadowNode : _outShadowNode;

                            top.sz = 1;
                            top.hashval = top.root.InShadow ? 1 : 0;
                            ++leavesCnt;
                            last = top;
                            break;
                        }

                        top.step = 1;
                        dataStack.Push(top);
                        break;
                    case 1:
                        //开始循环
                        if(top.childId >= 8 ) 
                            Debug.Log("Error!!!!!!!!");
                        stk.Add(top.childId);
                        top.step = 2;
                        dataStack.Push(top);
                        Data tmp = new Data(new Octree(), top.depth - 1, true, 1, 0, 0);
                        dataStack.Push(tmp);
                        break;
                    case 2:
                        //循环后半
                        var subTree = last.root;
                        var i = top.childId;
                        KeyValuePair<int, int> subVal = new KeyValuePair<int, int>(last.sz, last.hashval);
                        top.root.SubTree.Add(subTree);
                        //bool isCull = false;
                        //var subPos = GetWorldPositionByStack();
                        //if (subTree.InShadow)
                        //{
                        //    var length = GetSizeByDepth(stk.Count);
                        //    isCull = Physics.CheckBox(subPos, Vector3.one * 0.5f * length);
                        //}

                        var hashval = top.hashval;
                        top.hashval = (hashval + _hashCode[top.childId] * _primeList[subVal.Key%hashMax] % mod * subVal.Value % mod) % mod;
                        top.sz += subVal.Key;
                        stk.RemoveAt(stk.Count - 1);
                        if (top.flag)
                        {
                            if (top.root.SubTree[i].SubTree != null && top.root.SubTree[i].SubTree.Count > 0) top.flag = false;
                            if (i == 0)
                                top.root.InShadow = top.root.SubTree[i].InShadow;
                            //else if (isCull) continue;
                            else if (top.root.InShadow != top.root.SubTree[i].InShadow)
                                top.flag = false;
                        }

                        if (top.childId >= 7)
                        {
                            top.step = 3;
                            dataStack.Push(top);
                            break;
                        }

                        top.childId++;
                        top.step = 1;
                        dataStack.Push(top);
                        break;
                    case 3:
                        if (top.flag)
                        {
                            top.root.SubTree.Clear();

                            top.root = top.root.InShadow ? _inShadowNode : _outShadowNode;

                            top.sz = 1;
                            top.hashval = top.root.InShadow ? 1 : 0;
                            last = top;

                            mergeCnt += 8; 
                            break;
                        }

                        top.step = 4;
                        dataStack.Push(top);
                        break;
                    case 4:
                        if (_dic.ContainsKey(top.hashval))
                        {
                            top.root = _dic[top.hashval];
                            hashCnt += top.sz;
                        }
                        else _dic.Add(top.hashval, top.root);
                        last = top;
                        break;
                }
            }
            Debug.Log("try get : " + last.sz);
            dataStack.Clear();
        }
    }
}