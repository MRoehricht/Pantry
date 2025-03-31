//
//  Untitled.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI
import SwiftData

struct RecipeView: View {
    @Environment(\.modelContext) private var modelContext
    @Query private var recipes: [Recipe]
    @State private var selectedTags: [String] = []
    @State private var isFilterVisible: Bool = false
    @State private var searchText: String = ""

    var filteredRecipes: [Recipe] {
        let tagFilteredRecipes = selectedTags.isEmpty ? recipes : recipes.filter { recipe in
            !Set(selectedTags).isDisjoint(with: recipe.tags)
        }
        if searchText.isEmpty {
            return tagFilteredRecipes
        } else {
            return tagFilteredRecipes.filter { recipe in
                recipe.name.localizedCaseInsensitiveContains(searchText)
            }
        }
    }

    var allTags: [String] {
        Set(recipes.flatMap { $0.tags }).sorted()
    }

    var body: some View {
        NavigationView {
            VStack {
                if isFilterVisible {
                    TagFilterView(selectedTags: $selectedTags, allTags: allTags)
                        .padding()
                }

                ScrollView {
                    VStack {
                        ForEach(filteredRecipes) { recipe in
                            NavigationLink(destination: EditRecipeView(recipe: .constant(recipe))) {
                                RecipeCardView(recipe: recipe)
                            }
                        }
                    }
                }
                .refreshable {
                    // Pull-to-refresh action
                }
                .searchable(text: $searchText, placement: .navigationBarDrawer(displayMode: .automatic))
            }
            .navigationTitle("Rezepte")
            .toolbar {
                ToolbarItem(placement: .navigationBarLeading) {
                    Button(action: {
                        isFilterVisible.toggle()
                    }) {
                        Image(systemName: selectedTags.isEmpty ? "magnifyingglass" : "sparkle.magnifyingglass")
                    }
                }
                ToolbarItem(placement: .navigationBarTrailing) {
                    NavigationLink(destination: AddRecipeView()) {
                        Text("Hinzufügen")
                    }
                }
            }
        }
    }
}

#Preview {
    RecipeView()
        .modelContainer(for: [Recipe.self, Ingredient.self])
}
