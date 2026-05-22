namespace Shiki.Common.Event;

// TODO codegen

public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs eventArgs)
    where TEventArgs : EventArgs;

public delegate void TypedArgsEventHandler<in TSender>(TSender sender);

public delegate void TypedArgsEventHandler<in TSender, in T1>(TSender sender, T1 arg1)
    where T1 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2>(
    TSender sender, T1 arg1, T2 arg2)
    where T1 : allows ref struct
    where T2 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2, in T3>(
    TSender sender, T1 arg1, T2 arg2, T3 arg3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2, in T3, in T4>(
    TSender sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2, in T3, in T4, in T5>(
    TSender sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2, in T3, in T4, in T5, in T6>(
    TSender sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2, in T3, in T4, in T5, in T6, in T7>(
    TSender sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct;

public delegate void TypedArgsEventHandler<in TSender, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(
    TSender sender, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
    where T4 : allows ref struct
    where T5 : allows ref struct
    where T6 : allows ref struct
    where T7 : allows ref struct
    where T8 : allows ref struct;