# Avoid using HashSet.Add\HashSet.Remove after HashSet.Contains call

It is common to use HashSets to store items and check if an item is already in the set before adding or removing it. However, this pattern
of "Contains then Add" or "Contains then Remove" can lead to poor performance and is unnecessary:

```cs
HashSet<int> checkedNodeIds;
void Check(Node node) {
  if (!checkedNodeIds.Contains(node.Id))
    checkedNodeIds.Add(node.Id);
}
void Uncheck(Node node) {
  if (checkedNodeIds.Contains(node.Id))
    checkedNodeIds.Remove(node.Id);
}
```

In addition, you can also use the HashSet.Add method that returns a boolean indicating if the element was added to the set or not, in this
way you can check if it was added or removed from the HashSet:

```cs
HashSet<int> checkedNodeIds;
void Toggle(Node node) {
  if (!checkedNodeIds.Remove(node.Id))
    checkedNodeIds.Add(node.Id);
}
```