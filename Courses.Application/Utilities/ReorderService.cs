using Courses.Domain;
using Courses.Domain.Entities;

namespace Courses.Application.Utilities;

/// <summary>
/// Handles reordering of items implementing IDisplayOrder.
/// Uses a gap-based ordering scheme (10, 20, 30, ...) and renumbers when gaps are exhausted.
/// </summary>
public static class ReorderService
{
    private const int OrderGap = 10;

    /// <summary>
    /// Calculates the new DisplayOrder for an item being placed between two neighbors.
    /// If there is no integer gap between consecutive neighbors, all items are renumbered.
    /// </summary>
    /// <typeparam name="T">Entity type implementing BaseEntity + IDisplayOrder</typeparam>
    /// <param name="items">All sibling items in the collection, ordered by DisplayOrder</param>
    /// <param name="itemId">The item being moved</param>
    /// <param name="previousItemId">The item that should be before it (null = insert at beginning)</param>
    /// <param name="nextItemId">The item that should be after it (null = insert at end)</param>
    /// <returns>True if a full renumber was needed (caller should persist all items)</returns>
    public static bool Reorder<T>(IList<T> items, Guid itemId, Guid? previousItemId, Guid? nextItemId)
        where T : BaseEntity, IDisplayOrder
    {
        var item = items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item to reorder was not found.");

        if (previousItemId.HasValue && items.All(i => i.Id != previousItemId.Value))
            throw new InvalidOperationException("Previous item was not found.");

        if (nextItemId.HasValue && items.All(i => i.Id != nextItemId.Value))
            throw new InvalidOperationException("Next item was not found.");

        if (previousItemId.HasValue && nextItemId.HasValue && previousItemId == nextItemId)
            throw new InvalidOperationException("Previous and next items cannot be the same.");

        int? prevOrder = previousItemId.HasValue
            ? items.First(i => i.Id == previousItemId.Value).DisplayOrder
            : null;

        int? nextOrder = nextItemId.HasValue
            ? items.First(i => i.Id == nextItemId.Value).DisplayOrder
            : null;

        // Validate ordering: prev must come before next
        if (prevOrder.HasValue && nextOrder.HasValue && prevOrder.Value >= nextOrder.Value)
            throw new InvalidOperationException("Previous item must have a lower display order than next item.");

        int newOrder = CalculateNewOrder(prevOrder, nextOrder);

        if (newOrder != -1)
        {
            // There is a gap — simple placement
            item.DisplayOrder = newOrder;
            return false;
        }

        // No gap available — renumber everything
        // First, move the item to its logical position in the sorted list
        var sorted = items.Where(i => i.Id != itemId).OrderBy(i => i.DisplayOrder).ToList();

        int insertIndex;
        if (previousItemId is null)
        {
            insertIndex = 0;
        }
        else if (nextItemId is null)
        {
            insertIndex = sorted.Count;
        }
        else
        {
            insertIndex = sorted.FindIndex(i => i.Id == nextItemId.Value);
        }

        sorted.Insert(insertIndex, item);

        // Renumber all with fresh gaps
        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].DisplayOrder = (i + 1) * OrderGap;
        }

        return true; // All items were renumbered — caller should persist all
    }

    private static int CalculateNewOrder(int? prevOrder, int? nextOrder)
    {
        if (prevOrder is null && nextOrder is null)
        {
            // Only item, or no context — place at start
            return OrderGap;
        }

        if (prevOrder is null)
        {
            // Insert at the beginning, before nextOrder
            int candidate = nextOrder!.Value / 2;
            return candidate > 0 ? candidate : -1; // -1 signals renumber needed
        }

        if (nextOrder is null)
        {
            // Insert at the end, after prevOrder
            return prevOrder.Value + OrderGap;
        }

        // Insert between two items
        int gap = nextOrder.Value - prevOrder.Value;

        if (gap <= 1)
        {
            // No integer gap available — need renumber
            return -1;
        }

        return prevOrder.Value + (gap / 2);
    }
}
