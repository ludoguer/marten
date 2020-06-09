using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Marten.Linq;
using Marten.Linq.Fields;
using Marten.Linq.Model;
using Marten.Schema;
using Marten.Storage;
using Marten.Util;
using Marten.V4Internals.Linq;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;

namespace Marten.V4Internals
{
    public abstract class DocumentStorage<T, TId>: IDocumentStorage<T, TId>
    {
        private readonly IWhereFragment _defaultWhere;
        private readonly IQueryableDocument _document;

        public DocumentStorage(IQueryableDocument document)
        {
            _document = document;
            Fields = document;
            TableName = document.Table;

            _defaultWhere = document.DefaultWhereFragment();
        }

        public DbObjectName TableName { get; }
        public Type IdType => typeof(T);

        public Guid? VersionFor(T document, IMartenSession session)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            return session.Versions.VersionFor<T, TId>(Identity(document));
        }

        public abstract void Store(IMartenSession session, T document);
        public abstract void Store(IMartenSession session, T document, Guid? version);
        public abstract void Eject(IMartenSession session, T document);
        public abstract IStorageOperation Update(T document, IMartenSession session);
        public abstract IStorageOperation Insert(T document, IMartenSession session);
        public abstract IStorageOperation Upsert(T document, IMartenSession session);
        public abstract IStorageOperation Overwrite(T document, IMartenSession session);
        public abstract IStorageOperation DeleteForDocument(T document);
        public abstract IStorageOperation DeleteForWhere(IWhereFragment where);

        public IWhereFragment FilterDocuments(QueryModel model, IWhereFragment query)
        {
            return _document.FilterDocuments(model, query);
        }

        public IWhereFragment DefaultWhereFragment()
        {
            return _defaultWhere;
        }

        public IFieldMapping Fields { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TId Identity(T document);

        public abstract IStorageOperation DeleteForId(TId id);
        public abstract IQueryHandler<T> Load(TId id);
        public abstract IQueryHandler<T> LoadMany(TId[] ids);
    }
}