// **********************************************************************
//
// Copyright (c) 2003-2007 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

namespace IceInternal
{

    using System;
    using System.Net.Sockets;

    //
    // This is an internal exception used to communicate to the Ice run time that
    // an asynchronous read operation was aborted because the thread that initiated
    // it has exited or the socket was closed.
    //
    public class ReadAbortedException : Ice.SocketException
    {
        public ReadAbortedException()
        {
        }

        public ReadAbortedException(System.Exception ex) : base(ex)
        {
        }

        public ReadAbortedException(int error) : base(error)
        {
        }

        public ReadAbortedException(int error, System.Exception ex) : base(error, ex)
        {
        }
    }

    public interface Transceiver
    {
        Socket fd();

        //
        // Return true if read operations can be safely restarted.
        //
        bool restartable();

        //
        // Initialize the transceiver and block for the specified (non-zero) timeout. Initialization either
        // completes successfully in the given time, or an exception is raised.
        //
        void initialize(int timeout);

        //
        // Initialize the transceiver using asynchronous I/O. This method never blocks. Returns true
        // if initialization is complete, or false if an I/O request is pending. In the latter case,
        // the callback must invoke initialize again and repeat this process until it returns true.
        //
        bool initialize(AsyncCallback callback);

        void close();
        void shutdownWrite();
        void shutdownReadWrite();

        //
        // Write data.
        //
        // Returns true if all the data was written, false otherwise. If
        // timeout is -1, this operation will block until all the data is
        // written. If timeout is 0, it will return when the write can't
        // be completed without blocking. If the timeout is > 0, it will
        // block until all the data is written or the specified timeout
        // expires.
        //
        bool write(Buffer buf, int timeout);

        //
        // Read data.
        //
        // Returns true if all the requested data was read, false otherwise.
        // If timeout is -1, this operation will block until all the data
        // is read. If timeout is 0, it will return when the read can't be
        // completed without blocking. If the timeout is > 0, it will
        // block until all the data is read or the specified timeout
        // expires.
        //
        bool read(Buffer buf, int timeout);

        //
        // Read data asynchronously.
        //
        // The I/O request may complete synchronously, in which case endRead
        // will be invoked in the same thread as beginRead. The return value
        // from beginRead must be passed to endRead, along with the same buffer
        // object. The caller must check the buffer after endRead completes to
        // determine whether all of the requested data has been read.
        //
        // The read request is canceled upon the termination of the thread that
        // calls beginRead, or when the socket is closed. In this case endRead
        // raises ReadAbortedException.
        //
        IAsyncResult beginRead(Buffer buf, AsyncCallback callback, object state);
        void endRead(Buffer buf, IAsyncResult result);

        //
        // Write data asynchronously.
        //
        // The I/O request may complete synchronously, in which case endWrite
        // will be invoked in the same thread as beginWrite. The request
        // will be canceled upon the termination of the thread that calls beginWrite.
        //
        IAsyncResult beginWrite(Buffer buf, AsyncCallback callback, object state);
        void endWrite(Buffer buf, IAsyncResult result);

        string type();
        void checkSendSize(Buffer buf, int messageSizeMax);
    }

}
