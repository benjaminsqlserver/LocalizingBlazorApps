using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using MyFriendsApp.Data;

namespace MyFriendsApp
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ConDataContext context;
        private readonly NavigationManager navigationManager;

        public ConDataService(ConDataContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportFriendsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/friends/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/friends/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFriendsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/friends/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/friends/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFriendsRead(ref IQueryable<MyFriendsApp.Models.ConData.Friend> items);

        public async Task<IQueryable<MyFriendsApp.Models.ConData.Friend>> GetFriends(Query query = null)
        {
            var items = Context.Friends.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnFriendsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFriendGet(MyFriendsApp.Models.ConData.Friend item);
        partial void OnGetFriendByFriendId(ref IQueryable<MyFriendsApp.Models.ConData.Friend> items);


        public async Task<MyFriendsApp.Models.ConData.Friend> GetFriendByFriendId(long friendid)
        {
            var items = Context.Friends
                              .AsNoTracking()
                              .Where(i => i.FriendID == friendid);

 
            OnGetFriendByFriendId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFriendGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFriendCreated(MyFriendsApp.Models.ConData.Friend item);
        partial void OnAfterFriendCreated(MyFriendsApp.Models.ConData.Friend item);

        public async Task<MyFriendsApp.Models.ConData.Friend> CreateFriend(MyFriendsApp.Models.ConData.Friend friend)
        {
            OnFriendCreated(friend);

            var existingItem = Context.Friends
                              .Where(i => i.FriendID == friend.FriendID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Friends.Add(friend);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(friend).State = EntityState.Detached;
                throw;
            }

            OnAfterFriendCreated(friend);

            return friend;
        }

        public async Task<MyFriendsApp.Models.ConData.Friend> CancelFriendChanges(MyFriendsApp.Models.ConData.Friend item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFriendUpdated(MyFriendsApp.Models.ConData.Friend item);
        partial void OnAfterFriendUpdated(MyFriendsApp.Models.ConData.Friend item);

        public async Task<MyFriendsApp.Models.ConData.Friend> UpdateFriend(long friendid, MyFriendsApp.Models.ConData.Friend friend)
        {
            OnFriendUpdated(friend);

            var itemToUpdate = Context.Friends
                              .Where(i => i.FriendID == friend.FriendID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            Reset();

            Context.Attach(friend).State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFriendUpdated(friend);

            return friend;
        }

        partial void OnFriendDeleted(MyFriendsApp.Models.ConData.Friend item);
        partial void OnAfterFriendDeleted(MyFriendsApp.Models.ConData.Friend item);

        public async Task<MyFriendsApp.Models.ConData.Friend> DeleteFriend(long friendid)
        {
            var itemToDelete = Context.Friends
                              .Where(i => i.FriendID == friendid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnFriendDeleted(itemToDelete);

            Reset();

            Context.Friends.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFriendDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}