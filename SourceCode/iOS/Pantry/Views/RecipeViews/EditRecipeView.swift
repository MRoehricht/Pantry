//
//  EditRecipeView.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI
import SwiftData

struct EditRecipeView: View {
    @Binding var recipe: Recipe
    @State private var ingredientName: String = ""
    @State private var ingredientQuantity: String = ""
    @State private var selectedImage: UIImage?
    @State private var showImagePicker: Bool = false
    @State private var showDeleteConfirmation: Bool = false
    @State private var newTag: String = ""
    @Environment(\.modelContext) private var modelContext
    @Environment(\.presentationMode) private var presentationMode
    @Query private var allRecipes: [Recipe]

    var allTags: [String] {
        Set(allRecipes.flatMap { $0.tags }).sorted()
    }

    var filteredTags: [String] {
        allTags.filter { $0.lowercased().contains(newTag.lowercased()) && !newTag.isEmpty }
    }

    var body: some View {
        Form {
            Section(header: Text("Rezeptname")) {
                TextField("Name", text: $recipe.name)
            }
            Section(header: Text("Zutaten")) {
                if let ingredients = recipe.ingredients {
                    ForEach(ingredients.indices, id: \.self) { index in
                        HStack {
                            TextField("Zutat", text: Binding(
                                get: { ingredients[index].name },
                                set: { ingredients[index].name = $0 }
                            ))
                            TextField("Menge", text: Binding(
                                get: { ingredients[index].quantity },
                                set: { ingredients[index].quantity = $0 }
                            ))
                        }
                    }
                }
                HStack {
                    TextField("Zutat", text: $ingredientName)
                    TextField("Menge", text: $ingredientQuantity)
                    Button(action: addIngredient) {
                        Text("Hinzufügen")
                    }
                }
            }
            Section(header: Text("Tags")) {
                HStack {
                    TextField("Neuer Tag", text: $newTag)
                    Button(action: addTag) {
                        Text("Hinzufügen")
                    }
                }
                if !filteredTags.isEmpty {
                    VStack(alignment: .leading) {
                        Text("Vorschläge:")
                            .font(.caption)
                            .foregroundColor(.gray)
                        List(filteredTags, id: \.self) { tag in
                            Text(tag)
                                .onTapGesture {
                                    newTag = tag
                                    addTag()
                                }
                        }
                        .frame(maxHeight: 100)
                        .border(Color.gray, width: 1)
                    }
                }
                List {
                    ForEach(recipe.tags, id: \.self) { tag in
                        Text(tag)
                    }
                    .onDelete(perform: deleteTag)
                }
            }
            Section(header: Text("Bild")) {
                Button(action: {
                    showImagePicker = true
                }) {
                    Text("Bild auswählen")
                }
            }
            Section {
                Button(action: {
                    showDeleteConfirmation = true
                }) {
                    Text("Rezept löschen")
                        .foregroundColor(.red)
                }
            }
        }
        .navigationTitle("Rezept bearbeiten")
        .sheet(isPresented: $showImagePicker) {
            ImagePicker(image: $selectedImage)
        }
        .alert(isPresented: $showDeleteConfirmation) {
            Alert(
                title: Text("Rezept löschen"),
                message: Text("Möchten Sie dieses Rezept wirklich löschen?"),
                primaryButton: .destructive(Text("Löschen")) {
                    deleteRecipe()
                },
                secondaryButton: .cancel()
            )
        }
        .onChange(of: selectedImage) { newImage in
            if let newImage = newImage {
                recipe.imageData = newImage.jpegData(compressionQuality: 0.8)
            }
        }
        .onDisappear {
            saveRecipe()
        }
    }

    private func addIngredient() {
        let newIngredient = Ingredient(name: ingredientName, quantity: ingredientQuantity)
        recipe.ingredients?.append(newIngredient)
        ingredientName = ""
        ingredientQuantity = ""
    }

    private func addTag() {
        if !newTag.isEmpty && !recipe.tags.contains(newTag) {
                    recipe.tags.append(newTag)
                    newTag = ""
                }
    }
    
    private func deleteTag(at offsets: IndexSet) {
            recipe.tags.remove(atOffsets: offsets)
        }

    private func saveRecipe() {
        recipe.ingredients?.removeAll { $0.name.isEmpty && $0.quantity.isEmpty }
        do {
            try modelContext.save()
        } catch {
            print("Failed to save recipe: \(error)")
        }
    }

    private func deleteRecipe() {
        modelContext.delete(recipe)
        do {
            try modelContext.save()
            presentationMode.wrappedValue.dismiss()
        } catch {
            print("Failed to delete recipe: \(error)")
        }
    }
}

#Preview {
    EditRecipeView(recipe: .constant(Recipe(name: "Test", ingredients: [])))
        .modelContainer(for: [Recipe.self, Ingredient.self])
}
